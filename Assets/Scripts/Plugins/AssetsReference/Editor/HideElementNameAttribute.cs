using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Plugins.AssetsReference.Editor
{
    public class HideElementNameAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(HideElementNameAttribute))]
    public class HideElementNameAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            try
            {
                EditorGUI.ObjectField(rect, property, new GUIContent(string.Empty));
            }
            catch
            {
                EditorGUI.ObjectField(rect, property, label);
            }
        }
    }
#endif
}