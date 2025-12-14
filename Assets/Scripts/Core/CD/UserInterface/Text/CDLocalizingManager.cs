/*
    CDLocalizingManager v0.2

    작성 이유:
        - 순수 로직(System)과 Unity 진입점(Manager)을 분리해 테스트/확장이 용이하도록 개선.
*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CD
{
    public class CDLocalizingManager : MonoSingleton<CDLocalizingManager>
    {
        public event Action<SystemLanguage> OnLanguageChanged;

        public SystemLanguage CurLanguage { get; private set; } = SystemLanguage.Korean;

        private Dictionary<SystemLanguage, Dictionary<string, string>> languageTables;
        private Dictionary<string, string> curLanguageTable;

        protected override void Awake()
        {
            base.Awake();

            languageTables = CDLocalizingSystem.BuildLanguageTables();
            var initialLang = CDLocalizingSystem.ResolveLanguage(Application.systemLanguage, languageTables);
            ApplyLanguageInternal(initialLang);
        }

        public void SetLanguage(SystemLanguage lang)
        {
            if (lang == CurLanguage)
                return;

            if (languageTables == null || !languageTables.ContainsKey(lang))
            {
                Debug.LogWarning($"[Localization] {lang} 테이블이 없습니다.");
                return;
            }

            ApplyLanguageInternal(lang);
            OnLanguageChanged?.Invoke(CurLanguage);
        }

        public string GetText(string key)
        {
            if (curLanguageTable == null)
                return $"[NO_TABLE::{key}]";

            return curLanguageTable.TryGetValue(key, out var value)
                ? value
                : $"[MISSING_KEY::{key}]";
        }

        private void ApplyLanguageInternal(SystemLanguage lang)
        {
            CurLanguage = lang;
            curLanguageTable = languageTables[lang];
            Debug.Log($"[Localization] Language Applied: {CurLanguage}");
        }
    }
}
