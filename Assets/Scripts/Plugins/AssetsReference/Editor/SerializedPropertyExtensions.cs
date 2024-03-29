﻿#if UNITY_EDITOR
// --------------------------------------------------------------------------------------------------------------------
// <author>
//   HiddenMonk
//   http://answers.unity3d.com/users/496850/hiddenmonk.html
//   
//   Johannes Deml
//   send@johannesdeml.com
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Plugins.AssetsReference.Editor
{
    /// <summary>
    /// Extension class for SerializedProperties
    /// See also: http://answers.unity3d.com/questions/627090/convert-serializedproperty-to-custom-class.html
    /// </summary>
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Get the object the serialized property holds by using reflection
        /// </summary>
        /// <typeparam name="T">The object type that the property contains</typeparam>
        /// <param name="property"></param>
        /// <returns>Returns the object type T if it is the type the property actually contains</returns>
        public static T GetValue<T>(this SerializedProperty property, bool getFirst = false)
        {
            return GetNestedObject<T>(property.propertyPath, property.GetSerializedPropertyRootComponent(), getFirst: getFirst);
        }

        /// <summary>
        /// Set the value of a field of the property with the type T
        /// </summary>
        /// <typeparam name="T">The type of the field that is set</typeparam>
        /// <param name="property">The serialized property that should be set</param>
        /// <param name="value">The new value for the specified property</param>
        /// <returns>Returns if the operation was successful or failed</returns>
        public static bool SetValue<T>(this SerializedProperty property, T value)
        {
            object obj = property.GetSerializedPropertyRootComponent();

            //Iterate to parent object of the value, necessary if it is a nested object
            var fieldStructure = property.propertyPath.Split('.');
            for (var i = 0; i < fieldStructure.Length - 1; i++)
            {
                obj = GetFieldOrPropertyValue<object>(fieldStructure[i], obj);
            }

            var fieldName = fieldStructure.Last();

            return SetFieldOrPropertyValue(fieldName, obj, value);
        }

        /// <summary>
        /// Get the component of a serialized property
        /// </summary>
        /// <param name="property">The property that is part of the component</param>
        /// <returns>The root component of the property</returns>
        public static Component GetSerializedPropertyRootComponent(this SerializedProperty property)
        {
            return (Component)property.serializedObject.targetObject;
        }

        /// <summary>
        /// Iterates through objects to handle objects that are nested in the root object
        /// </summary>
        /// <typeparam name="T">The type of the nested object</typeparam>
        /// <param name="path">Path to the object through other properties e.g. PlayerInformation.Health</param>
        /// <param name="obj">The root object from which this path leads to the property</param>
        /// <param name="includeAllBases">Include base classes and interfaces as well</param>
        /// <returns>Returns the nested object casted to the type T</returns>
        public static T GetNestedObject<T>(string path,
                                           object obj,
                                           bool includeAllBases = true,
                                           bool getFirst = false)
        {
            var split = path.Split('.');
            for (var i = 0; i < split.Length; i++)
            {
                var part = split[i];

                if (part == "Array" && obj is IList array)
                {
                    var indexString = split[++i]
                                      .Replace("data[", string.Empty)
                                      .Replace("]", string.Empty);
                    var index = int.Parse(indexString);
                    var value = array[index];

                    obj = value;

                    if (i < split.Length - 1)
                    {
                        part = split[++i];
                    }

                    if (obj is T result)
                    {
                        return result;
                    }
                }

                obj = GetFieldOrPropertyValue<object>(part, obj, includeAllBases);

                if (getFirst && obj != null)
                {
                    return (T)obj;
                }
            }

            return (T)obj;
        }

        public static T GetFieldOrPropertyValue<T>(string fieldName,
                                                   object obj,
                                                   bool includeAllBases = false,
                                                   BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                                                           BindingFlags.NonPublic)
        {
            var field = obj.GetType().GetField(fieldName, bindings);
            if (field != null) return (T)field.GetValue(obj);

            var property = obj.GetType().GetProperty(fieldName, bindings);
            if (property != null) return (T)property.GetValue(obj, null);

            if (includeAllBases)
            {
                foreach (var type in GetBaseClassesAndInterfaces(obj.GetType()))
                {
                    field = type.GetField(fieldName, bindings);
                    if (field != null) return (T)field.GetValue(obj);

                    property = type.GetProperty(fieldName, bindings);
                    if (property != null) return (T)property.GetValue(obj, null);
                }
            }

            return default(T);
        }

        public static bool SetFieldOrPropertyValue(string fieldName,
                                                   object obj,
                                                   object value,
                                                   bool includeAllBases = false,
                                                   BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                                                           BindingFlags.NonPublic)
        {
            var field = obj.GetType().GetField(fieldName, bindings);
            if (field != null)
            {
                field.SetValue(obj, value);
                return true;
            }

            var property = obj.GetType().GetProperty(fieldName, bindings);
            if (property != null)
            {
                property.SetValue(obj, value, null);
                return true;
            }

            if (!includeAllBases)
            {
                return false;
            }

            foreach (var type in GetBaseClassesAndInterfaces(obj.GetType()))
            {
                field = type.GetField(fieldName, bindings);
                if (field != null)
                {
                    field.SetValue(obj, value);
                    return true;
                }

                property = type.GetProperty(fieldName, bindings);
                if (property == null)
                {
                    continue;
                }

                property.SetValue(obj, value, null);
                return true;
            }

            return false;
        }

        public static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type, bool includeSelf = false)
        {
            var allTypes = new List<Type>();

            if (includeSelf) allTypes.Add(type);

            if (type.BaseType == typeof(object))
            {
                allTypes.AddRange(type.GetInterfaces());
            }
            else
            {
                allTypes.AddRange(
                                  Enumerable
                                      .Repeat(type.BaseType, 1)
                                      .Concat(type.GetInterfaces())
                                      .Concat(type.BaseType.GetBaseClassesAndInterfaces())
                                      .Distinct());
            }

            return allTypes;
        }
    }
}
#endif