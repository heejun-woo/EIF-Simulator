using Opc.Ua;
using Opc.Ua.Server;


namespace EIF_Simulator.OpcUa
{
    public class NodeManager : CustomNodeManager2
    {
        public FolderState db1;
        public NodeStateCollection Variables;

        public Dictionary<string, object> _memory = new Dictionary<string, object>();
        public NodeManager(IServerInternal server, ApplicationConfiguration config)
            : base(server, config, "http://fakeplc")
        {
        }
        public NodeState? findNode(string name)
        {
            foreach (var node in Variables)
            {
                if (node.DisplayName.Text == name)
                {
                    return node;
                }
            }
            return null;
        }

        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            Variables = new NodeStateCollection();

            return Variables;
        }

        public override void CreateAddressSpace(
        IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            base.CreateAddressSpace(externalReferences);

            // Objects 폴더 레퍼런스 확보
            if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out IList<IReference> refs))
            {
                externalReferences[ObjectIds.ObjectsFolder] = refs = new List<IReference>();
            }

            // 📁 DB1 생성
            db1 = new FolderState(null)
            {
                NodeId = new NodeId("DB1", NamespaceIndex),
                BrowseName = new QualifiedName("DB1", NamespaceIndex),
                DisplayName = "DB1",
                TypeDefinitionId = ObjectTypeIds.FolderType
            };

            // 🔥 Objects → DB1 연결
            refs.Add(new NodeStateReference(
                ReferenceTypeIds.Organizes,
                false,
                db1.NodeId));

            db1.AddReference(
                ReferenceTypeIds.Organizes,
                true,
                ObjectIds.ObjectsFolder);

            AddPredefinedNode(SystemContext, db1);


            StartLoop();

        }


        private BaseDataVariableState CreateVariable(NodeState parent, string name, NodeId dataType, object defaultValue)
        {
            // 메모리 초기화
            _memory[name] = defaultValue;

            if (defaultValue is not Array)
            {
                var variable = new BaseDataVariableState(parent)
                {
                    SymbolicName = name,
                    NodeId = new NodeId(name, NamespaceIndex),
                    BrowseName = new QualifiedName(name, NamespaceIndex),
                    DisplayName = name,

                    DataType = dataType,
                    Value = defaultValue,

                    ValueRank = ValueRanks.Scalar,
                    ArrayDimensions = null,

                    AccessLevel = AccessLevels.CurrentReadOrWrite,
                    UserAccessLevel = AccessLevels.CurrentReadOrWrite,
                    TypeDefinitionId = VariableTypeIds.BaseDataVariableType
                };

                parent.AddChild(variable);
                AddPredefinedNode(SystemContext, variable);

                variable.OnSimpleWriteValue = (ISystemContext ctx, NodeState node, ref object val) =>
                {
                    _memory[name] = val;
                    Console.WriteLine($"{node.DisplayName} = {val}");
                    return ServiceResult.Good;
                };

                return variable;
            }
            else
            {
                Array arr_defaultValue = (Array)defaultValue;
                var variable = new BaseDataVariableState(parent)
                {
                    SymbolicName = name,
                    NodeId = new NodeId(name, NamespaceIndex),
                    BrowseName = new QualifiedName(name, NamespaceIndex),
                    DisplayName = name,

                    DataType = dataType,
                    Value = arr_defaultValue,

                    ValueRank = ValueRanks.OneDimension,
                    ArrayDimensions = new uint[] { (uint)arr_defaultValue.Length },

                    AccessLevel = AccessLevels.CurrentReadOrWrite,
                    UserAccessLevel = AccessLevels.CurrentReadOrWrite,
                    TypeDefinitionId = VariableTypeIds.BaseDataVariableType
                };

                for (int i = 0; i < arr_defaultValue.Length; i++)
                {
                    int index = i; // 클로저 문제 방지 위해 지역 변수로 저장

                    _memory[$"{name}[{index}]"] = arr_defaultValue.GetValue(index);

                    var item = new BaseDataVariableState(parent)
                    {
                        SymbolicName = $"{name}[{index}]",
                        NodeId = new NodeId($"{name}[{index}]", NamespaceIndex),
                        BrowseName = new QualifiedName($"{name}[{index}]", NamespaceIndex),
                        DisplayName = $"{name}[{index}]",
                        DataType = dataType,
                        Value = arr_defaultValue.GetValue(index),
                        ValueRank = ValueRanks.Scalar,
                        ArrayDimensions = null,
                        AccessLevel = AccessLevels.CurrentReadOrWrite,
                        UserAccessLevel = AccessLevels.CurrentReadOrWrite,
                        TypeDefinitionId = VariableTypeIds.BaseDataVariableType
                    };

                    item.OnSimpleReadValue = (ISystemContext ctx, NodeState node, ref object val) =>
                    {
                        val = arr_defaultValue.GetValue(index);
                        return ServiceResult.Good;
                    };

                    item.OnSimpleWriteValue = (ISystemContext ctx, NodeState node, ref object val) =>
                    {
                        arr_defaultValue.SetValue(val, index);
                        node.ClearChangeMasks(SystemContext, false);

                        variable.Value = arr_defaultValue;
                        variable.ClearChangeMasks(SystemContext, false);

                        return ServiceResult.Good;
                    };

                    variable.AddChild(item);
                    AddPredefinedNode(SystemContext, item);

                }

                parent.AddChild(variable);
                AddPredefinedNode(SystemContext, variable);

                variable.OnSimpleWriteValue = (ISystemContext ctx, NodeState node, ref object val) =>
                {
                    _memory[name] = val;
                    Console.WriteLine($"{node.DisplayName} = {val}");
                    return ServiceResult.Good;
                };

                return variable;
            }

        }

        public BaseDataVariableState? AddVariable(NodeState parent, string name, NodeId dataType, object defaultValue)
        {
            var variable = CreateVariable(parent, name, dataType, defaultValue);
            Variables.Add(variable);

            return variable;
        }

        public BaseDataVariableState? AddVariable(string name, NodeId dataType, object defaultValue)
        {
            var variable = CreateVariable(db1, name, dataType, defaultValue);
            Variables.Add(variable);

            return variable;
        }

        public void StartLoop()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    foreach (var node in PredefinedNodes.Values)
                    {
                        if (node is BaseDataVariableState varNode)
                        {
                            string key = varNode.NodeId.Identifier.ToString();

                            if (_memory.ContainsKey(key))
                            {
                                varNode.Value = _memory[key];
                                varNode.Timestamp = DateTime.UtcNow;
                                varNode.ClearChangeMasks(SystemContext, false);
                            }
                        }
                    }

                    await Task.Delay(100);
                }
            });
        }


    }
}
