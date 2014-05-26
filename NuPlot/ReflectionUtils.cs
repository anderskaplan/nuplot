using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace NuPlot
{
    internal static class ReflectionUtils
    {
        #region interface IGetValue

        /// <summary>
        /// Interface representing a field or property of an object (or similar); a piece of data which can be requested from an object.
        /// </summary>
        private interface IGetValue
        {
            /// <summary>
            /// Data type of the value.
            /// </summary>
            Type ValueType { get; }

            /// <summary>
            /// Extract the value from an object.
            /// </summary>
            object GetValue(object o);
        }

        #endregion

        #region private class PropertyGetter : IGetValue

        /// <summary>
        /// An implementation of IGetValue wrapping a property.
        /// </summary>
        private class PropertyGetter : IGetValue
        {
            private readonly MethodInfo _getter;

            public PropertyGetter(MethodInfo getter)
            {
                _getter = getter;
            }

            public Type ValueType
            {
                get { return _getter.ReturnType; }
            }

            public object GetValue(object o)
            {
                return _getter.Invoke(o, new object[] { });
            }
        }

        #endregion
            
        #region private class FieldGetter : IGetValue

        /// <summary>
        /// An implementation of IGetValue wrapping a field.
        /// </summary>
        private class FieldGetter : IGetValue
        {
            private readonly FieldInfo _fieldInfo;

            public FieldGetter(FieldInfo fieldInfo)
            {
                _fieldInfo = fieldInfo;
            }

            public Type ValueType
            {
                get { return _fieldInfo.FieldType; }
            }

            public object GetValue(object o)
            {
                return _fieldInfo.GetValue(o);
            }
        }

        #endregion

        /// <summary>
        /// Transform an array or enumeration of objects into world points.
        /// X and Y values are pulled off the objects based on the xValuePath and yValuePath parameters.
        /// </summary>
        public static IEnumerable<WorldPoint> GetPoints(object itemsSource, string xValuePath, string yValuePath)
        {
            if (itemsSource == null || xValuePath == null || yValuePath == null)
            {
                return new WorldPoint[] { };
            }

            // case 1. ItemsSource is an Array
            var itemsArray = itemsSource as Array;
            if (itemsArray != null)
            {
                var elementType = itemsArray.GetType().GetElementType();
                return GetPoints(itemsArray, elementType, xValuePath, yValuePath);
            }

            // case 2. ItemsSource is an IEnumerable
            var itemsEnum = itemsSource as IEnumerable;
            if (itemsEnum != null)
            {
                var firstElement = First(itemsEnum);
                if (firstElement != null)
                {
                    var elementType = firstElement.GetType();
                    return GetPoints(itemsEnum, elementType, xValuePath, yValuePath);
                }
                else
                {
                    return new WorldPoint[] { };
                }
            }

            // no match -- fail.
            Trace.WriteLine(string.Format("Binding error: the type '{0}' cannot be used as an ItemsSource. Please see the NuPlot documentation for valid types.", itemsSource.GetType()));
            return new WorldPoint[] { };
        }

        /// <summary>
        /// Transform objects into world points.
        /// X and Y values are pulled off the objects based on the xValuePath and yValuePath parameters.
        /// </summary>
        private static IEnumerable<WorldPoint> GetPoints(IEnumerable items, Type elementType, string xValuePath, string yValuePath)
        {
            var xGetter = CreateGetter(elementType, xValuePath);
            var yGetter = CreateGetter(elementType, yValuePath);
            if (xGetter != null && yGetter != null)
            {
                return GetPoints(items, xGetter, yGetter);
            }
            else
            {
                return new WorldPoint[] { };
            }
        }

        /// <summary>
        /// Transform objects into world points using IGetValue getters for x and y.
        /// </summary>
        private static IEnumerable<WorldPoint> GetPoints(IEnumerable items, IGetValue xGetter, IGetValue yGetter)
        {
            foreach (var o in items)
            {
                yield return new WorldPoint(xGetter.GetValue(o), yGetter.GetValue(o));
            }
        }

        private static object First(IEnumerable items)
        {
            var enumerator = items.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }
            else
            {
                return null;
            }
        }

        private static IGetValue CreateGetter(Type type, string valuePath)
        {
            var property = type.GetProperty(valuePath, BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.CanRead)
            {
                return new PropertyGetter(property.GetGetMethod());
            }

            var field = type.GetField(valuePath, BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                return new FieldGetter(field);
            }

            Trace.WriteLine(string.Format("Binding error: cannot access property or field '{0}' on type '{1}'.", valuePath, type));
            return null;
        }
    }
}
