using System;
using UnityEditor;
using UnityEngine;

namespace Plugins.AssetsReference
{
    [Serializable]
    public class SettingValueBase
    {
        public string Id;
    }

    [Serializable]
    public abstract class SettingValue<T> : SettingValueBase
    {
        public T Value;
    }
    
    [Serializable]
    public class BoolSetting : SettingValue<bool> { }
    
    [Serializable]
    public class IntSetting : SettingValue<int> { }
    
    [Serializable]
    public class FloatSetting : SettingValue<float> { }
    
    [Serializable]
    public class StringSetting : SettingValue<string> { }

    [Serializable]
    public class Vector2Setting : SettingValue<Vector2> { }

    [Serializable]
    public class Vector3Setting : SettingValue<Vector3> { }

    [Serializable]
    public class Vector4Setting : SettingValue<Vector4> { }

    [Serializable]
    public class Vector2IntSetting : SettingValue<Vector2Int> { }

    [Serializable]
    public class Vector3IntSetting : SettingValue<Vector3Int> { }
    
    [Serializable]
    public class RectSetting : SettingValue<Rect> { }
    
    [Serializable]
    public class BoundsSetting : SettingValue<Bounds> { }
    
    [Serializable]
    public class ColorSetting : SettingValue<Color> { }

    [Serializable]
    public class LayerMaskSetting : SettingValue<LayerMask> { }
    
    [Serializable]
    public class AnimationCurveSetting : SettingValue<AnimationCurve> { }

    [Serializable]
    public class GradientSetting : SettingValue<Gradient> { }

    [Serializable]
    public class SpriteSetting : SettingValue<Sprite> { }

    [Serializable]
    public class MaterialSetting : SettingValue<Material> { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SettingValueBase))]
    public class SettingValueDrawer : PropertyDrawer
    {
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

            DrawMain(firstLineRect, property);

            EditorGUI.indentLevel = indent;
        }

        private void DrawMain(Rect rect, SerializedProperty property)
        {
            var propertyKey = property.FindPropertyRelative("Id");
            var propertyValue = property.FindPropertyRelative("Value");

            rect.width = (rect.width - 2 * space) / 2 - 20;
            DrawProperty(rect, propertyKey);

            rect.x += rect.width + space;
            DrawProperty(rect, propertyValue);

            rect.x += rect.width + space;
            if (DrawButton(new Rect(rect.x, rect.y, 40, 18), "D"))
            {
                property.DeleteCommand();
            }
        }

        private void DrawProperty(Rect rect, SerializedProperty property)
        {
            if (property != null)
            {
                EditorGUI.PropertyField(rect, property, GUIContent.none);
            }
        }

        private bool DrawButton(Rect rect, string title)
        {
            return GUI.Button(rect, title);
        }
    }
#endif
}