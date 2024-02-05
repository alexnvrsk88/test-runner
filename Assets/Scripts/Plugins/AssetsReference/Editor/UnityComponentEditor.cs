#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Plugins;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.AssetsReference.Editor
{
    [CustomEditor(typeof(UnityComponent))]
    public class UnityComponentEditor : UnityEditor.Editor
    {
        private static readonly Dictionary<string, string> _shortNames = new()
        {
            [nameof(Boolean)] = "bool",
            [nameof(Int32)] = "int",
            [nameof(Single)] = "float",
            [nameof(String)] = "string"
        };

        private UnityComponent _component;
        private static List<Type> _settingTypes;
        private static Dictionary<Type, Type> _settingValueTypes;

        private static List<Type> SettingTypes => _settingTypes ??= GetSupportedTypes();

        private static Dictionary<Type, Type> SettingValueTypes => _settingValueTypes ??= GetSupportedSettingsTypes();

        public override void OnInspectorGUI()
        {
            _component ??= target as UnityComponent;
            if (_component == null)
            {
                base.OnInspectorGUI();
                return;
            }

            if (_component is IUnityComponent { IsAlive: true } unityComponent)
            {
                var hasActiveController = string.IsNullOrEmpty(unityComponent.ActiveController) == false;
                
                GUILayout.Space(4);
                GUILayout.BeginHorizontal();
                
                GUILayout.Label("Active controller: ");
                
                var prevColor = GUI.color;
                GUI.color = hasActiveController ? Color.green : Color.gray;
                GUILayout.Label(hasActiveController ? $"{unityComponent.ActiveController}" : "None");
                GUI.color = prevColor;
                
                GUILayout.FlexibleSpace();
                
                GUILayout.EndHorizontal();
                GUILayout.Space(8);
            }

            base.OnInspectorGUI();

            for (var i = _component.EditorSettings.Count - 1; i >= 0; i--)
            {
                var setting = _component.EditorSettings[i];
                if (setting == null)
                {
                    _component.EditorSettings.RemoveAt(i);
                }
            }

            if (GUILayout.Button("Add Empty Reference"))
            {
                _component.EditorReferences.Add(new Reference());
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Add Empty Setting"))
            {
                ShowSettingTypesToAdd();
            }

            if (GUILayout.Button("Generate"))
            {
                GenerateCodeStrings();
            }
        }

        private void ShowSettingTypesToAdd()
        {
            var menu = new GenericMenu();

            foreach (var supportedType in SettingTypes)
            {
                var typeName = supportedType.Name;
                typeName = _shortNames.GetValueOrDefault(typeName, typeName);
                
                menu.AddItem(new GUIContent(typeName), false, () => Add(supportedType));
            }

            menu.ShowAsContext();
        }

        private void GenerateCodeStrings()
        {
            var defineStringBuilder = new StringBuilder();
            var getReferenceStringBuilder = new StringBuilder();

            foreach (var reference in _component.EditorReferences)
            {
                var variableName = "_" + char.ToLower(reference.Id[0]) + reference.Id[1..];
                var variableType = reference.Target.GetType().Name;
                var getType = variableType;

                if (reference.Target is UnityComponent)
                {
                    variableType = nameof(IUnityComponent);
                }

                var defineCode = $"private {variableType} {variableName};";
                defineStringBuilder.AppendLine(defineCode);

                var referenceCode = $"{variableName} = View.Get<{getType}>(\"{reference.Id}\");";
                getReferenceStringBuilder.AppendLine(referenceCode);
            }

            foreach (var setting in _component.EditorSettings)
            {
                var variableName = "_" + char.ToLower(setting.Id[0]) + setting.Id[1..];
                var variableType = setting.GetType().BaseType.GenericTypeArguments.First().Name;
                variableType = _shortNames.GetValueOrDefault(variableType, variableType);

                var defineCode = $"private readonly {variableType} {variableName};";
                defineStringBuilder.AppendLine(defineCode);

                var settingCode = $"{variableName} = View.GetSetting<{variableType}>(\"{setting.Id}\");";
                getReferenceStringBuilder.AppendLine(settingCode);
            }

            var window = EditorWindow.GetWindow<GeneratedCodeWindow>();
            window.Setup(defineStringBuilder.ToString(), getReferenceStringBuilder.ToString());
            window.Focus();
        }

        private void Add(Type supportedType)
        {
            var requiredType = SettingValueTypes[supportedType];
            var settingsValueBase = ReflectionUtils.Create<SettingValueBase>(requiredType, genericArguments: supportedType);

            _component.EditorSettings.Add(settingsValueBase);

            EditorUtility.SetDirty(target);
        }

        private static List<Type> GetSupportedTypes()
        {
            return SettingValueTypes.Keys.ToList();
        }

        private static Dictionary<Type, Type> GetSupportedSettingsTypes()
        {
            var types = Assembly.GetExecutingAssembly().ExportedTypes;
            var supportedTypes = types
                                 .Where(IsSupportedType)
                                 .ToList();
            var dictionary = supportedTypes
                             .Where(IsSupportedSettingType)
                             .ToDictionary(k => k.BaseType.GenericTypeArguments.First());

            return dictionary;
        }

        private static bool IsSupportedType(Type type)
        {
            var baseType = typeof(SettingValueBase);
            return type.IsAbstract == false && type != baseType && baseType.IsAssignableFrom(type);
        }

        private static bool IsSupportedSettingType(Type type)
        {
            return type.BaseType is { IsGenericType: true } && type.BaseType.GenericTypeArguments.Length > 0;
        }
    }
}
#endif