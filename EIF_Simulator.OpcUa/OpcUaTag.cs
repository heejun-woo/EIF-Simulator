using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIF_Simulator.OpcUa
{
    public class OpcUaTag
    {
        public string Name { get; }

        public Type ValueType { get; }

        public object DefaultValue { get; }

        public bool IsArray =>
            ValueType.IsArray;

        public OpcUaTag(
            string name,
            object defaultValue)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Tag name cannot be empty.",
                    nameof(name));
            }

            DefaultValue =
                defaultValue ??
                throw new ArgumentNullException(
                    nameof(defaultValue));

            Name = name;

            ValueType =
                defaultValue.GetType();
        }
    }
}