using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plugins;
using UnityEngine;

#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine.UI;
#endif

namespace Plugins.AssetsReference
{
    [Serializable]
    internal class Reference
    {
        [SerializeField] private UnityEngine.Object _source;
        [SerializeField] private UnityEngine.Object _target;
        [SerializeField] private string _id;
        [SerializeField] private int _componentId;
        public string Id => _id;
        public UnityEngine.Object Target => _target;
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Reference))]
    public class ReferencePropertyDrawer : PropertyDrawer 
    {
        private readonly List<Type> _componentsTypes = new List<Type>();
        private readonly List<Component> _components = new List<Component>();
        private readonly Type[] _priorityComponents = new Type[]{typeof(UnityComponent), typeof(TextMeshProUGUI), typeof(Button), typeof(Image)};

        private const float space = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) 
        {
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var firstLineRect = new Rect(
                x: rect.x,
                y: rect.y,
                width: rect.width,
                height: EditorGUIUtility.singleLineHeight
            );
            
            DrawMainProperties(firstLineRect, property);

            EditorGUI.indentLevel = indent;
        }
       
        private void DrawMainProperties(Rect rect, SerializedProperty property)
        {
            var propertyId = property.FindPropertyRelative("_id");
            var propertyElement = property.FindPropertyRelative("_source");
            var propertyTarget = property.FindPropertyRelative("_target");
            var propertyComponentId = property.FindPropertyRelative("_componentId");
            
            _componentsTypes.Clear();
            _components.Clear();

            GetComponents(propertyElement);
            
            //Draw object selector--------
            rect.width = (rect.width - 2 * space) / 3;
            
            EditorGUI.BeginChangeCheck();
            DrawProperty(rect, propertyElement);
            if (EditorGUI.EndChangeCheck())
            {
                if (propertyElement != null)
                {
                    if (string.IsNullOrEmpty(propertyId.stringValue))
                    {
                        propertyId.stringValue = propertyElement.objectReferenceValue.name;
                    }
                    
                    GetComponents(propertyElement);

                    var componentId = 0;
                    foreach (var type in _priorityComponents)
                    {
                        var index = _componentsTypes.FindIndex(t => t == type);
                        if (index >= 0)
                        {
                            componentId = index;
                            break;
                        }
                    }
                    
                    propertyComponentId.intValue = componentId;
                }
            }
            
            //Draw dropdown--------
            if (_components.Count > 0)
            {
                rect.x += rect.width + space;
                var propertyRect = new Rect(rect.x,rect.y,120,18);
                var typesOptions = _componentsTypes.Select(t => t.Name).ToArray();
                propertyComponentId.intValue = EditorGUI.Popup(propertyRect, propertyComponentId.intValue, typesOptions);
                propertyTarget.objectReferenceValue = propertyComponentId.intValue < _components.Count ? _components[propertyComponentId.intValue] : null;
            }
            
            //Draw name to call--------
            rect.x += 120 + space;
            DrawProperty(rect, propertyId);

            rect.x += rect.width + space;
            if (DrawButton(new Rect(rect.x,rect.y,40,18), "x"))
            {
                property.DeleteCommand();
            }
            
            _componentsTypes.Clear();
            _components.Clear();
        }

        private void GetComponents(SerializedProperty propertyElement)
        {
            if (propertyElement.objectReferenceValue != null)
            {
                switch (propertyElement.objectReferenceValue)
                {
                    case Component component:
                        _components.Add(component);
                        break;
                    case GameObject go:
                        go.GetComponents(_components);
                        break;
                }

                for (int i = 0; i < _components.Count; i++)
                {
                    _componentsTypes.Add(_components[i].GetType());
                }
            }
        }

        private bool DrawProperty(Rect rect, SerializedProperty property)
        {
            return EditorGUI.PropertyField(rect, property, GUIContent.none);
        }

        private bool DrawButton(Rect rect, string title)
        {
            return GUI.Button(rect, title);
        }
    }
#endif
}