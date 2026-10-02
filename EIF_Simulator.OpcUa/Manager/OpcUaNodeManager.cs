using Opc.Ua;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;

namespace EIF_Simulator.OpcUa
{
    public class OpcUaNodeManager : CustomNodeManager2
    {
        private readonly OpcUaBindingManager _bindingManager;

        private readonly Dictionary<string, BaseDataVariableState>
            _nodes =
                new(StringComparer.OrdinalIgnoreCase);

        private FolderState? _root;

        public OpcUaNodeManager(
            IServerInternal server,
            ApplicationConfiguration configuration,
            OpcUaBindingManager bindingManager)
            : base(
                server,
                configuration,
                "http://eif-simulator/opcua")
        {
            _bindingManager =
                bindingManager;

            _bindingManager.ValueChanged +=
                OnBindingValueChanged;
        }

        public override void CreateAddressSpace(
            IDictionary<NodeId, IList<IReference>>
                externalReferences)
        {
            base.CreateAddressSpace(
                externalReferences);

            if (!externalReferences.TryGetValue(
                    ObjectIds.ObjectsFolder,
                    out IList<IReference>? references))
            {
                references =
                    new List<IReference>();

                externalReferences[
                    ObjectIds.ObjectsFolder] =
                    references;
            }

            _root =
                new FolderState(null)
                {
                    NodeId =
                        new NodeId(
                            "Tags",
                            NamespaceIndex),

                    BrowseName =
                        new QualifiedName(
                            "Tags",
                            NamespaceIndex),

                    DisplayName =
                        new LocalizedText(
                            "Tags"),

                    TypeDefinitionId =
                        ObjectTypeIds.FolderType
                };

            references.Add(
                new NodeStateReference(
                    ReferenceTypeIds.Organizes,
                    false,
                    _root.NodeId));

            _root.AddReference(
                ReferenceTypeIds.Organizes,
                true,
                ObjectIds.ObjectsFolder);

            AddPredefinedNode(
                SystemContext,
                _root);

            //
            // BindingManager에 미리 등록된 태그들을
            // OPC UA Node로 생성한다.
            //
            foreach (OpcUaTag tag in
                     _bindingManager.GetTags())
            {
                CreateTagNode(tag);
            }
        }

        private void CreateTagNode(
            OpcUaTag tag)
        {
            if (_root == null)
                return;

            if (_nodes.ContainsKey(
                    tag.Name))
            {
                return;
            }

            object? value =
                _bindingManager.GetValue(
                    tag.Name);

            if (value == null)
                return;

            NodeId dataType =
                GetDataType(
                    tag.ValueType);

            var variable =
                new BaseDataVariableState(
                    _root)
                {
                    SymbolicName =
                        tag.Name,

                    NodeId =
                        new NodeId(
                            tag.Name,
                            NamespaceIndex),

                    BrowseName =
                        new QualifiedName(
                            tag.Name,
                            NamespaceIndex),

                    DisplayName =
                        new LocalizedText(
                            tag.Name),

                    DataType =
                        dataType,

                    Value =
                        new Variant(
                            value),

                    ValueRank =
                        tag.IsArray
                            ? ValueRanks.OneDimension
                            : ValueRanks.Scalar,

                    AccessLevel =
                        AccessLevels.CurrentReadOrWrite,

                    UserAccessLevel =
                        AccessLevels.CurrentReadOrWrite,

                    TypeDefinitionId =
                        VariableTypeIds
                            .BaseDataVariableType
                };

            //
            // 배열 태그
            //
            if (value is Array array)
            {
                variable.ArrayDimensions =
                    new[]
                    {
                        (uint)array.Length
                    }.ToArrayOf();
            }

            //
            // OPC UA Client Read / Write
            //
            variable.OnSimpleReadValue =
                OnReadValue;

            variable.OnSimpleWriteValue =
                OnWriteValue;

            _root.AddChild(
                variable);

            AddPredefinedNode(
                SystemContext,
                variable);

            _nodes[tag.Name] =
                variable;
        }

        /// <summary>
        /// OPC UA Client가 값을 읽을 때 호출된다.
        /// </summary>
        private ServiceResult OnReadValue(
            ISystemContext context,
            NodeState node,
            ref Variant value)
        {
            string tagName =
                node.NodeId.IdentifierAsString;

            object? currentValue =
                _bindingManager.GetValue(
                    tagName);

            if (currentValue == null)
            {
                return new ServiceResult(
                    StatusCodes.BadNotFound);
            }

            value =
                new Variant(
                    currentValue);

            return ServiceResult.Good;
        }

        /// <summary>
        /// OPC UA Client가 값을 쓸 때 호출된다.
        /// </summary>
        private ServiceResult OnWriteValue(
            ISystemContext context,
            NodeState node,
            ref Variant value)
        {
            string tagName =
                node.NodeId.IdentifierAsString;

            object? rawValue =
                value.Value;

            if (rawValue == null)
            {
                return new ServiceResult(
                    StatusCodes.BadTypeMismatch);
            }

            bool result =
                _bindingManager.SetValue(
                    tagName,
                    rawValue);

            if (!result)
            {
                return new ServiceResult(
                    StatusCodes.BadNotFound);
            }

            return ServiceResult.Good;
        }

        /// <summary>
        /// Simulator 내부에서 BindingManager.SetValue()가
        /// 호출됐을 때 OPC UA Node 값을 갱신한다.
        /// </summary>
        private void OnBindingValueChanged(
            string tagName,
            object value)
        {
            if (!_nodes.TryGetValue(
                    tagName,
                    out BaseDataVariableState? node))
            {
                return;
            }

            node.Value =
                new Variant(
                    value);

            node.Timestamp =
                DateTime.UtcNow;

            //
            // OPC UA Subscription에 변경을 알린다.
            //
            node.ClearChangeMasks(
                SystemContext,
                false);
        }

        /// <summary>
        /// C# 타입을 OPC UA DataType으로 변환한다.
        /// 배열이면 ElementType을 기준으로 판단한다.
        /// </summary>
        private static NodeId GetDataType(
            Type type)
        {
            Type valueType =
                type.IsArray
                    ? type.GetElementType()!
                    : type;

            if (valueType == typeof(bool))
                return DataTypeIds.Boolean;

            if (valueType == typeof(sbyte))
                return DataTypeIds.SByte;

            if (valueType == typeof(byte))
                return DataTypeIds.Byte;

            if (valueType == typeof(short))
                return DataTypeIds.Int16;

            if (valueType == typeof(ushort))
                return DataTypeIds.UInt16;

            if (valueType == typeof(int))
                return DataTypeIds.Int32;

            if (valueType == typeof(uint))
                return DataTypeIds.UInt32;

            if (valueType == typeof(long))
                return DataTypeIds.Int64;

            if (valueType == typeof(ulong))
                return DataTypeIds.UInt64;

            if (valueType == typeof(float))
                return DataTypeIds.Float;

            if (valueType == typeof(double))
                return DataTypeIds.Double;

            if (valueType == typeof(string))
                return DataTypeIds.String;

            if (valueType == typeof(DateTime))
                return DataTypeIds.DateTime;

            throw new NotSupportedException(
                $"OPC UA data type is not supported: {valueType.FullName}");
        }

        protected override void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                _bindingManager.ValueChanged -=
                    OnBindingValueChanged;
            }

            base.Dispose(
                disposing);
        }
    }
}