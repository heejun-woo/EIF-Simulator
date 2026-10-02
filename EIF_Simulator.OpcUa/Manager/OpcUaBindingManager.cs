using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIF_Simulator.OpcUa
{
    public class OpcUaBindingManager
    {
        private readonly Dictionary<string, OpcUaTag>
            _tags =
                new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, object>
            _values =
                new(StringComparer.OrdinalIgnoreCase);

        private readonly object _lock =
            new();

        public event Action<string, object>?
            ValueChanged;

        public void AddTag(
            string tagName,
            object defaultValue)
        {
            lock (_lock)
            {
                if (_tags.ContainsKey(tagName))
                    return;

                var tag =
                    new OpcUaTag(
                        tagName,
                        defaultValue);

                _tags[tagName] =
                    tag;

                _values[tagName] =
                    defaultValue;
            }
        }

        public IReadOnlyList<OpcUaTag> GetTags()
        {
            lock (_lock)
            {
                return _tags
                    .Values
                    .ToList();
            }
        }

        public bool Contains(
            string tagName)
        {
            lock (_lock)
            {
                return _tags.ContainsKey(
                    tagName);
            }
        }

        public object? GetValue(
            string tagName)
        {
            lock (_lock)
            {
                if (_values.TryGetValue(
                        tagName,
                        out object? value))
                {
                    return value;
                }
            }

            return null;
        }

        public T? GetValue<T>(
            string tagName)
        {
            object? value =
                GetValue(tagName);

            if (value == null)
                return default;

            if (value is T typedValue)
                return typedValue;

            return (T)Convert.ChangeType(
                value,
                typeof(T));
        }

        public bool SetValue(
            string tagName,
            object value)
        {
            object? changedValue;

            lock (_lock)
            {
                if (!_tags.TryGetValue(
                        tagName,
                        out OpcUaTag? tag))
                {
                    return false;
                }

                object convertedValue =
                    ConvertValue(
                        value,
                        tag.ValueType);

                if (_values.TryGetValue(
                        tagName,
                        out object? oldValue) &&
                    ValuesEqual(
                        oldValue,
                        convertedValue))
                {
                    return true;
                }

                _values[tagName] =
                    convertedValue;

                changedValue =
                    convertedValue;
            }

            ValueChanged?.Invoke(
                tagName,
                changedValue);

            return true;
        }

        public bool SetValue(
            string tagName,
            int index,
            object value)
        {
            object? changedValue;

            lock (_lock)
            {
                if (!_values.TryGetValue(
                        tagName,
                        out object? current))
                {
                    return false;
                }

                if (current is not Array array)
                    return false;

                if (index < 0 ||
                    index >= array.Length)
                {
                    return false;
                }

                Type? elementType =
                    array.GetType()
                        .GetElementType();

                if (elementType == null)
                    return false;

                object convertedValue =
                    ConvertValue(
                        value,
                        elementType);

                if (Equals(
                        array.GetValue(index),
                        convertedValue))
                {
                    return true;
                }

                array.SetValue(
                    convertedValue,
                    index);

                changedValue =
                    array;
            }

            ValueChanged?.Invoke(
                tagName,
                changedValue);

            return true;
        }

        public bool ClearValue(
            string tagName)
        {
            object? newValue;

            lock (_lock)
            {
                if (!_values.TryGetValue(
                        tagName,
                        out object? current))
                {
                    return false;
                }

                if (current is string)
                {
                    newValue =
                        string.Empty;
                }
                else if (current is Array array)
                {
                    Array.Clear(
                        array,
                        0,
                        array.Length);

                    newValue =
                        array;
                }
                else
                {
                    Type type =
                        current.GetType();

                    newValue =
                        type.IsValueType
                            ? Activator.CreateInstance(type)
                            : null;
                }

                _values[tagName] =
                    newValue!;
            }

            ValueChanged?.Invoke(
                tagName,
                newValue!);

            return true;
        }

        private static object ConvertValue(
            object value,
            Type targetType)
        {
            if (targetType.IsInstanceOfType(value))
                return value;

            return Convert.ChangeType(
                value,
                targetType);
        }

        private static bool ValuesEqual(
            object? left,
            object? right)
        {
            if (ReferenceEquals(
                    left,
                    right))
            {
                return true;
            }

            if (left == null ||
                right == null)
            {
                return false;
            }

            if (left is Array leftArray &&
                right is Array rightArray)
            {
                if (leftArray.Length !=
                    rightArray.Length)
                {
                    return false;
                }

                for (int i = 0;
                     i < leftArray.Length;
                     i++)
                {
                    if (!Equals(
                            leftArray.GetValue(i),
                            rightArray.GetValue(i)))
                    {
                        return false;
                    }
                }

                return true;
            }

            return Equals(
                left,
                right);
        }
    }
}
