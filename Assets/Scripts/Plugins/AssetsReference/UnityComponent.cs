using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Plugins.AssetsReference
{
    /// <summary>
    /// Класс прокси для получение ссылок на геймобъекты, компоненты на сцене или префабе
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnityComponent : MonoBehaviour, IUnityComponent
    {
        [SerializeField] [HideInInspector] private int _id;
        [SerializeField] private List<Reference> _references = new();
        [SerializeReference] private List<SettingValueBase> _settings = new();

        private RectTransform _rectTransform;
        private Transform _transform;

        int IUnityComponent.Id => _id;
        GameObject IUnityComponent.GameObject => gameObject;
        Transform IUnityComponent.Transform => _transform ??= transform;
        RectTransform IUnityComponent.RectTransform => _rectTransform ??= transform as RectTransform;
        bool IUnityComponent.IsAlive => gameObject != null;
        string IUnityComponent.ActiveController { get; set; }

        /// <summary>
        /// Get component by type and id
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T IUnityComponent.Get<T>(string id)
        {
            for (var i = 0; i < _references.Count; i++)
            {
                var reference = _references[i];
                if (reference.Id != id)
                {
                    continue;
                }

                return reference.Target as T;
            }

            return default;
        }

        /// <summary>
        /// Get settings by type and id
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T IUnityComponent.GetSetting<T>(string id)
        {
            for (var i = 0; i < _settings.Count; i++)
            {
                var settingsReference = _settings[i];
                if (settingsReference.Id == id && settingsReference is SettingValue<T> setting)
                {
                    return setting.Value;
                }
            }

            return default;
        }

        public override int GetHashCode()
        {
            return _id;
        }

#if UNITY_EDITOR
        internal List<Reference> EditorReferences => _references;
        internal List<SettingValueBase> EditorSettings => _settings;

        private void OnValidate()
        {
            if (_id != 0)
            {
                return;
            }

            _id = Guid.NewGuid().GetHashCode();
            EditorUtility.SetDirty(gameObject);
        }
#endif
    }
}