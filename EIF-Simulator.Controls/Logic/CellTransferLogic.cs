
using EIF_Simulator.Controls.Manager;
using EIF_Simulator.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace EIF_Simulator.Controls
{
    public class CellTransferLogic : IPlcLogic
    {
        public class LoadTarget
        {
            public string NodeId { get; }
            public string NextNodeId { get; }

            public LoadTarget(
                string nodeId,
                string nextNodeId)
            {
                NodeId = nodeId;
                NextNodeId = nextNodeId;
            }
        }

        private readonly CellUnloadControl _unload;
        private readonly string _unloadNodeId;
        private readonly string _unloadNextNodeId;
        private readonly List<LoadTarget> _targets;
        private readonly int _magazineSize;
        private readonly PlcContext _context;

        private readonly Stopwatch _timer =
            Stopwatch.StartNew();

        private int _processing;

        private NodeRegistry Nodes => _context.Nodes;
        private CarrierManager Carriers => _context.CarrierManager;
        private CarrierHistoryManager History =>
            _context.CarrierHistory;
        private CarrierRouteManager Routes =>
            _context._routeManager;

        // 기존 1:1 생성자 유지
        public CellTransferLogic(
            CellUnloadControl unload,
            string unloadNodeId,
            string loadNodeId,
            string unloadNextNodeId,
            string loadNextNodeId,
            int magazineSize,
            int transferIntervalMs,
            PlcContext context)
            : this(
                unload,
                unloadNodeId,
                unloadNextNodeId,
                new[]
                {
                    new LoadTarget(
                        loadNodeId,
                        loadNextNodeId)
                },
                magazineSize,
                transferIntervalMs,
                context)
        {
        }

        // 신규 1:N 생성자
        public CellTransferLogic(
            CellUnloadControl unload,
            string unloadNodeId,
            string unloadNextNodeId,
            IEnumerable<LoadTarget> targets,
            int magazineSize,
            int transferIntervalMs,
            PlcContext context)
        {
            _unload = unload;
            _unloadNodeId = unloadNodeId;
            _unloadNextNodeId = unloadNextNodeId;
            _targets = targets.ToList();
            _magazineSize = magazineSize;
            _context = context;

            if (_targets.Count == 0)
                throw new ArgumentException(
                    "Load target is required.");

            if (_targets.Any(x =>
                string.IsNullOrWhiteSpace(x.NodeId)))
            {
                throw new ArgumentException(
                    "Invalid load target.");
            }

            if (_targets.Select(x => x.NodeId)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count() != _targets.Count)
                throw new ArgumentException(
                    "Duplicate load target.");

            if (magazineSize <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(magazineSize));

            _unload.TransferIntervalMs =
                transferIntervalMs;
        }

        public void Scan(PlcBindingManager manager)
        {
            if (!_unload.TransferEnabled)
                return;

            if (_timer.ElapsedMilliseconds <
                _unload.TransferIntervalMs)
                return;

            if (Interlocked.CompareExchange(
                    ref _processing, 1, 0) != 0)
                return;

            _timer.Restart();

            try
            {
                ProcessTransfer();
            }
            catch
            {
                Interlocked.Exchange(
                    ref _processing, 0);
                throw;
            }
        }

        private void ProcessTransfer()
        {
            var dispatcher = _unload.Dispatcher;

            try
            {
                dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        try
                        {
                            ProcessTransferOnUiThread();
                        }
                        finally
                        {
                            Finish();
                        }
                    }));
            }
            catch
            {
                Finish();
                throw;
            }
        }

        private void ProcessTransferOnUiThread()
        {
            // 1. Load Full 캐리어 배출은 독립적으로 처리
            for (int i = _targets.Count - 1; i >= 0; i--)
            {
                var target = _targets[i];

                var session =
                    Carriers.FindSessionAtNode(target.NodeId);

                if (session == null)
                    continue;

                var carrier = session.Carrier;

                if (carrier.CurrentCellCount >= carrier.MaxCellCount)
                {
                    if (!string.IsNullOrWhiteSpace(target.NextNodeId))
                    {
                        Routes.SetDestination(
                            carrier,
                            target.NextNodeId);
                    }
                }
            }

            // 2. Load 캐리어 정렬
            bool loadReady = PrepareLoadCarriers();

            // 3. Unload 캐리어 확인
            var sourceSession =
                Carriers.FindSessionAtNode(_unloadNodeId);

            if (sourceSession == null)
                return;

            // 4. Unload Empty 배출
            if (sourceSession.Carrier.CurrentCellCount == 0)
            {
                if (!string.IsNullOrWhiteSpace(_unloadNextNodeId))
                {
                    Routes.SetDestination(
                        sourceSession.Carrier,
                        _unloadNextNodeId);
                }

                return;
            }

            if (!loadReady)
                return;

            // 5. 기존 Load 세션 확인 및 ExecuteTransfer

            // 5. Load 세션 조회
            var targetSessions =
                new List<CarrierSession>();

            foreach (var target in _targets)
            {
                if (Nodes.Get(target.NodeId)
                    is not CellLoadControl)
                    return;

                var session =
                    Carriers.FindSessionAtNode(
                        target.NodeId);

                if (session == null)
                    return;

                targetSessions.Add(session);
            }

            // 6. 셀 이동
            ExecuteTransfer(
                sourceSession,
                targetSessions);
        }


        private bool PrepareLoadCarriers()
        {
            if (_targets.Count < 2)
                return true;

            string load1 = _targets[0].NodeId;
            string load2 = _targets[1].NodeId;

            var session1 = Carriers.FindSessionAtNode(load1);
            var session2 = Carriers.FindSessionAtNode(load2);

            Debug.WriteLine(
                $"Load1={load1}, Carrier={session1?.Carrier.Id}, " +
                $"Load2={load2}, Carrier={session2?.Carrier.Id}");

            if (session1 != null && session2 == null)
            {
                bool result = Routes.SetDestination(
                    session1.Carrier,
                    load2);

                Debug.WriteLine(
                    $"Move {session1.Carrier.Id}: " +
                    $"{load1} -> {load2}, Result={result}");

                return false;
            }

            return session1 != null && session2 != null;
        }

        private void ExecuteTransfer(
            CarrierSession sourceSession,
            List<CarrierSession> targetSessions)
        {
            // Dispatcher 실행 시점에 세션 재확인
            if (!ReferenceEquals(
                    Carriers.FindSessionAtNode(
                        _unloadNodeId),
                    sourceSession))
                return;

            for (int i = 0; i < _targets.Count; i++)
            {
                if (!ReferenceEquals(
                        Carriers.FindSessionAtNode(
                            _targets[i].NodeId),
                        targetSessions[i]))
                    return;
            }

            var source = sourceSession.Carrier;
            var targets = targetSessions
                .Select(x => x.Carrier)
                .ToList();

            int count = targets.Count;

            int[] capacities = targets
                .Select(x => Math.Max(
                    0,
                    Math.Min(
                        _magazineSize,
                        x.MaxCellCount -
                        x.CurrentCellCount)))
                .ToArray();

            // 어느 한쪽도 이동할 수 없다면
            // 그룹 전체가 대기한다.
            if (capacities.Any(x => x == 0))
            {
                // 이미 Full인 Load가 있으면
                // 그룹을 함께 배출한다.
                if (targets.Any(x =>
                    x.CurrentCellCount >= x.MaxCellCount))
                {
                    DepartTargets(targetSessions);
                }

                return;
            }

            int remaining =
                source.CurrentCellCount;

            if (remaining <= 0)
            {
                DepartAll(
                    sourceSession,
                    targetSessions);
                return;
            }

            int[] amounts = new int[count];

            // 각 Load에 동일한 수량 우선 배분
            int equalCount = Math.Min(
                remaining / count,
                capacities.Min());

            for (int i = 0; i < count; i++)
            {
                amounts[i] = equalCount;
                remaining -= equalCount;
            }

            // 홀수 등 남은 수량은
            // 첫 번째 Load부터 1개씩 배분
            for (int i = 0;
                 i < count && remaining > 0;
                 i++)
            {
                if (amounts[i] < capacities[i])
                {
                    amounts[i]++;
                    remaining--;
                }
            }

            int total = amounts.Sum();

            if (total <= 0)
                return;

            // 이동 수량을 모두 확정한 후 반영
            source.CurrentCellCount -= total;

            for (int i = 0; i < count; i++)
            {
                targets[i].CurrentCellCount +=
                    amounts[i];
            }

            // Unload Empty: 전부 함께 출발
            if (source.CurrentCellCount == 0)
            {
                History.Add(
                    source.Id,
                    string.Empty,
                    string.Empty,
                    CarrierHistoryType
                        .CellTransfer.ToString(),
                    "CellTransfer : Empty");

                DepartAll(
                    sourceSession,
                    targetSessions);

                return;
            }

            // Load Full: Load 그룹 함께 출발
            if (targets.Any(x =>
                x.CurrentCellCount >= x.MaxCellCount))
            {
                DepartTargets(targetSessions);
            }
        }

        private void DepartAll(
            CarrierSession source,
            List<CarrierSession> targets)
        {
            Routes.SetDestination(
                source.Carrier,
                _unloadNextNodeId);

            DepartTargets(targets);
        }

        private void DepartTargets(
            List<CarrierSession> targets)
        {
            // 앞쪽 Load부터 출발 요청
            for (int i = targets.Count - 1; i >= 0; i--)
            {
                var carrier = targets[i].Carrier;
                string nextNodeId = _targets[i].NextNodeId;

                if (string.IsNullOrWhiteSpace(nextNodeId))
                    continue;

                bool result = Routes.SetDestination(
                    carrier,
                    nextNodeId);

                Debug.WriteLine(
                    $"Load Depart: " +
                    $"Carrier={carrier.Id}, " +
                    $"Current={carrier.CurrentNode?.Id}, " +
                    $"Next={nextNodeId}, " +
                    $"Result={result}");
            }
        }

        private void Finish()
        {
            Interlocked.Exchange(
                ref _processing, 0);
        }
    }
}

