#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Plugins.AssetsReference.Editor
{
    public sealed class GeneratedCodeWindow : EditorWindow
    {
        private string _variablesPart;
        private string _gettingPart;
        private Vector2 _scroll;

        public void Setup(string variablesPart, string settingsPart)
        {
            _variablesPart = variablesPart;
            _gettingPart = settingsPart;
        }
        
        private void OnGUI()
        {
            _scroll = GUILayout.BeginScrollView(_scroll);
            
            GUILayout.Label("Generated code", EditorStyles.boldLabel);
            
            GUILayout.Space(10);
            EditorGUILayout.SelectableLabel(_variablesPart, EditorStyles.textArea, GUILayout.Height(200), GUILayout.ExpandHeight(true));
            if (GUILayout.Button("Copy"))
            {
                EditorGUIUtility.systemCopyBuffer = _variablesPart;
            }

            GUILayout.Space(10);
            EditorGUILayout.SelectableLabel(_gettingPart, EditorStyles.textArea, GUILayout.Height(200), GUILayout.ExpandHeight(true));
            if (GUILayout.Button("Copy"))
            {
                EditorGUIUtility.systemCopyBuffer = _gettingPart;
            }
            
            GUILayout.Space(10);
            if (GUILayout.Button("Copy all"))
            {
                EditorGUIUtility.systemCopyBuffer = _variablesPart + "\n" + _gettingPart;
            }
            
            GUILayout.EndScrollView();
        }
    }
}
#endif