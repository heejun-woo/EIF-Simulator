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

        private readonly Dictionary<string, FolderState>
    _folders =
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
            IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            base.CreateAddressSpace(externalReferences);

            foreach (OpcUaTag tag in _bindingManager.GetTags())
            {
                CreateTagNode(
                    tag,
                    externalReferences);
            }
        }

        private FolderState GetOrCreateTagFolder(
    string tagName,
    IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            string[] parts =
                tagName.Split('.');

            if (parts.Length <= 1)
                throw new InvalidOperationException(
                    $"Invalid tag name: {tagName}");

            FolderState? parent = null;
            string currentPath = string.Empty;

            for (int i = 0; i < parts.Length - 1; i++)
            {
                string folderName = parts[i];

                currentPath =
                    string.IsNullOrEmpty(currentPath)
                        ? folderName
                        : $"{currentPath}.{folderName}";

                if (_folders.TryGetValue(
                        currentPath,
                        out FolderState? existing))
                {
                    parent = existing;
                    continue;
                }

                var folder =
                    new FolderState(parent)
                    {
                        NodeId =
                            new NodeId(
                                $"Folder.{currentPath}",
                                NamespaceIndex),

                        BrowseName =
                            new QualifiedName(
                                folderName,
                                NamespaceIndex),

                        DisplayName =
                            new LocalizedText(
                                folderName),

                        TypeDefinitionId =
                            ObjectTypeIds.FolderType
                    };

                if (parent == null)
                {
                    // LGES를 Objects 바로 아래에 연결
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

                    references.Add(
                        new NodeStateReference(
                            ReferenceTypeIds.Organizes,
                            false,
                            folder.NodeId));

                    folder.AddReference(
                        ReferenceTypeIds.Organizes,
                        true,
                        ObjectIds.ObjectsFolder);
                }
                else
                {
                    parent.AddChild(folder);
                }

                AddPredefinedNode(
                    SystemContext,
                    folder);

                _folders[currentPath] = folder;
                parent = folder;
            }

            return parent!;
        }


        private FolderState GetOrCreateTagFolder(
    string tagName)
        {
            if (_root == null)
                throw new InvalidOperationException(
                    "Root folder is not initialized.");

            string[] parts =
                tagName.Split('.');

            // 마지막은 실제 Tag Node이므로 제외
            // LGES.CommMng.Check.E_CommCheck
            // → LGES / CommMng / Check
            if (parts.Length <= 1)
                return _root;

            FolderState parent = _root;
            string currentPath = string.Empty;

            for (int i = 0; i < parts.Length - 1; i++)
            {
                string folderName = parts[i];

                currentPath =
                    string.IsNullOrEmpty(currentPath)
                        ? folderName
                        : $"{currentPath}.{folderName}";

                if (_folders.TryGetValue(
                        currentPath,
                        out FolderState? existing))
                {
                    parent = existing;
                    continue;
                }

                var folder =
                    new FolderState(parent)
                    {
                        NodeId =
                            new NodeId(
                                $"Folder.{currentPath}",
                                NamespaceIndex),

                        BrowseName =
                            new QualifiedName(
                                folderName,
                                NamespaceIndex),

                        DisplayName =
                            new LocalizedText(
                                folderName),

                        TypeDefinitionId =
                            ObjectTypeIds.FolderType
                    };

                parent.AddChild(folder);

                AddPredefinedNode(
                    SystemContext,
                    folder);

                _folders[currentPath] = folder;

                parent = folder;
            }

            return parent;
        }

        private void CreateTagNode(
            OpcUaTag tag,
            IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            if (tag.IsArray)
            {
                CreateArrayTagNode(
                    tag,
                    externalReferences);

                return;
            }

            CreateSingleTagNode(
                tag,
                externalReferences);
        }

        private void CreateArrayTagNode(
    OpcUaTag tag,
    IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            object? value =
                _bindingManager.GetValue(tag.Name);

            if (value is not Array array)
                return;

            FolderState parent =
                GetOrCreateTagFolder(
                    tag.Name,
                    externalReferences);

            Type? elementType =
                tag.ValueType.GetElementType();

            if (elementType == null)
                return;

            NodeId dataType =
                GetDataType(elementType);

            string displayName =
                tag.Name.Split('.').Last();

            //
            // 배열 전체 Node
            //
            var arrayNode =
                new BaseDataVariableState(parent)
                {
                    SymbolicName = displayName,

                    NodeId =
                        new NodeId(
                            tag.Name,
                            NamespaceIndex),

                    BrowseName =
                        new QualifiedName(
                            displayName,
                            NamespaceIndex),

                    DisplayName =
                        new LocalizedText(
                            displayName),

                    DataType = dataType,

                    Value =
                        new Variant(array),

                    ValueRank =
                        ValueRanks.OneDimension,

                    ArrayDimensions =
                        new[]
                        {
                    (uint)array.Length
                        }.ToArrayOf(),

                    AccessLevel =
                        AccessLevels.CurrentReadOrWrite,

                    UserAccessLevel =
                        AccessLevels.CurrentReadOrWrite,

                    TypeDefinitionId =
                        VariableTypeIds.BaseDataVariableType
                };

            arrayNode.OnSimpleReadValue =
                OnReadValue;

            arrayNode.OnSimpleWriteValue =
                OnWriteValue;

            parent.AddChild(arrayNode);

            AddPredefinedNode(
                SystemContext,
                arrayNode);

            _nodes[tag.Name] = arrayNode;

            //
            // 배열 Element Node
            //
            for (int i = 0;
                 i < array.Length;
                 i++)
            {
                int opcIndex =
                    i + 1;

                string elementName =
                    $"{displayName}[{opcIndex}]";

                string elementNodeId =
                    $"{tag.Name}[{opcIndex}]";

                object? elementValue =
                    array.GetValue(i);

                if (elementValue == null)
                    continue;

                var elementNode =
                    new BaseDataVariableState(arrayNode)
                    {
                        SymbolicName =
                            elementName,

                        NodeId =
                            new NodeId(
                                elementNodeId,
                                NamespaceIndex),

                        BrowseName =
                            new QualifiedName(
                                elementName,
                                NamespaceIndex),

                        DisplayName =
                            new LocalizedText(
                                elementName),

                        DataType =
                            dataType,

                        Value =
                            new Variant(
                                elementValue),

                        ValueRank =
                            ValueRanks.Scalar,

                        AccessLevel =
                            AccessLevels.CurrentReadOrWrite,

                        UserAccessLevel =
                            AccessLevels.CurrentReadOrWrite,

                        TypeDefinitionId =
                            VariableTypeIds.BaseDataVariableType
                    };

                elementNode.OnSimpleReadValue =
                    OnReadValue;

                elementNode.OnSimpleWriteValue =
                    OnWriteValue;

                //
                // 핵심:
                // 배열 Node 아래에 Element Node를 Child로 등록
                //
                arrayNode.AddChild(
                    elementNode);

                AddPredefinedNode(
                    SystemContext,
                    elementNode);

                _nodes[elementNodeId] =
                    elementNode;
            }
        }

        private void CreateSingleTagNode(
    OpcUaTag tag,
    IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            if (_nodes.ContainsKey(tag.Name))
                return;

            object? value =
                _bindingManager.GetValue(tag.Name);

            if (value == null)
                return;

            FolderState parent =
                GetOrCreateTagFolder(
                    tag.Name,
                    externalReferences);

            string displayName =
                tag.Name.Split('.').Last();

            var variable =
                new BaseDataVariableState(parent)
                {
                    SymbolicName = displayName,

                    NodeId =
                        new NodeId(
                            tag.Name,
                            NamespaceIndex),

                    BrowseName =
                        new QualifiedName(
                            displayName,
                            NamespaceIndex),

                    DisplayName =
                        new LocalizedText(displayName),

                    DataType =
                        GetDataType(tag.ValueType),

                    Value =
                        new Variant(value),

                    ValueRank =
                        ValueRanks.Scalar,

                    AccessLevel =
                        AccessLevels.CurrentReadOrWrite,

                    UserAccessLevel =
                        AccessLevels.CurrentReadOrWrite,

                    TypeDefinitionId =
                        VariableTypeIds.BaseDataVariableType
                };

            variable.OnSimpleReadValue =
                OnReadValue;

            variable.OnSimpleWriteValue =
                OnWriteValue;

            parent.AddChild(variable);

            AddPredefinedNode(
                SystemContext,
                variable);

            _nodes[tag.Name] = variable;
        }

        private void CreateArrayTagNodes(
    OpcUaTag tag,
    IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            object? value =
                _bindingManager.GetValue(tag.Name);

            if (value is not Array array)
                return;

            FolderState parent =
                GetOrCreateTagFolder(
                    tag.Name,
                    externalReferences);

            Type? elementType =
                tag.ValueType.GetElementType();

            if (elementType == null)
                return;

            NodeId dataType =
                GetDataType(elementType);

            string baseName =
                tag.Name.Split('.').Last();

            for (int i = 0; i < array.Length; i++)
            {
                // 외부 OPC UA에서는 1부터 시작
                int tagIndex = i + 1;

                string nodeName =
                    $"{baseName}[{tagIndex}]";

                string nodeId =
                    $"{tag.Name}[{tagIndex}]";

                if (_nodes.ContainsKey(nodeId))
                    continue;

                object? elementValue =
                    array.GetValue(i);

                if (elementValue == null)
                    continue;

                var variable =
                    new BaseDataVariableState(parent)
                    {
                        SymbolicName = nodeName,

                        NodeId =
                            new NodeId(
                                nodeId,
                                NamespaceIndex),

                        BrowseName =
                            new QualifiedName(
                                nodeName,
                                NamespaceIndex),

                        DisplayName =
                            new LocalizedText(
                                nodeName),

                        DataType = dataType,

                        Value =
                            new Variant(
                                elementValue),

                        ValueRank =
                            ValueRanks.Scalar,

                        AccessLevel =
                            AccessLevels.CurrentReadOrWrite,

                        UserAccessLevel =
                            AccessLevels.CurrentReadOrWrite,

                        TypeDefinitionId =
                            VariableTypeIds.BaseDataVariableType
                    };

                variable.OnSimpleReadValue =
                    OnReadValue;

                variable.OnSimpleWriteValue =
                    OnWriteValue;

                parent.AddChild(variable);

                AddPredefinedNode(
                    SystemContext,
                    variable);

                _nodes[nodeId] = variable;
            }
        }


        private static bool TryParseArrayNodeId(
    string nodeId,
    out string tagName,
    out int index)
        {
            tagName = nodeId;
            index = -1;

            int openIndex =
                nodeId.LastIndexOf('[');

            int closeIndex =
                nodeId.LastIndexOf(']');

            if (openIndex < 0 ||
                closeIndex <= openIndex)
            {
                return false;
            }

            string indexText =
                nodeId.Substring(
                    openIndex + 1,
                    closeIndex - openIndex - 1);

            if (!int.TryParse(
                    indexText,
                    out int opcIndex))
            {
                return false;
            }

            if (opcIndex <= 0)
                return false;

            tagName =
                nodeId.Substring(
                    0,
                    openIndex);

            // OPC UA 표시 인덱스는 1부터,
            // C# 배열은 0부터
            index = opcIndex - 1;

            return true;
        }


        /// <summary>
        /// OPC UA Client가 값을 읽을 때 호출된다.
        /// </summary>
        private ServiceResult OnReadValue(
            ISystemContext context,
            NodeState node,
            ref Variant value)
        {
            string nodeId =
                node.NodeId.IdentifierAsString;

            if (TryParseArrayNodeId(
                    nodeId,
                    out string tagName,
                    out int index))
            {
                object? rawValue =
                    _bindingManager.GetValue(
                        tagName);

                if (rawValue is not Array array)
                {
                    return new ServiceResult(
                        StatusCodes.BadNotFound);
                }

                if (index < 0 ||
                    index >= array.Length)
                {
                    return new ServiceResult(
                        StatusCodes.BadIndexRangeInvalid);
                }

                object? elementValue =
                    array.GetValue(index);

                if (elementValue == null)
                {
                    return new ServiceResult(
                        StatusCodes.BadNoData);
                }

                value =
                    new Variant(
                        elementValue);

                return ServiceResult.Good;
            }

            object? currentValue =
                _bindingManager.GetValue(
                    nodeId);

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
            string nodeId =
                node.NodeId.IdentifierAsString;

            object? rawValue =
                value.Value;

            if (rawValue == null)
            {
                return new ServiceResult(
                    StatusCodes.BadTypeMismatch);
            }

            if (TryParseArrayNodeId(
                    nodeId,
                    out string tagName,
                    out int index))
            {
                bool arrayResult =
                    _bindingManager.SetValue(
                        tagName,
                        index,
                        rawValue);

                if (!arrayResult)
                {
                    return new ServiceResult(
                        StatusCodes.BadTypeMismatch);
                }

                return ServiceResult.Good;
            }

            bool result =
                _bindingManager.SetValue(
                    nodeId,
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