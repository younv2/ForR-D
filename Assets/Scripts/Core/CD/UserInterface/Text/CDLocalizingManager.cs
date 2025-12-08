/*
    CDLocalizingSystem + Manager v0.2

    작성 이유:
        - 순수 로직(System)과 Unity 진입점(Manager)을 분리해 테스트/확장이 용이하도록 개선.

    특징:
        - CDLocalizingSystem은 언어 테이블 관리와 키 조회만 담당.
        - CDLocalizingManager(MonoSingleton)는 초기화, 언어 변경 이벤트, Unity 의존 기능만 담당.
*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CD
{
    public class CDLocalizingSystem
    {
        public SystemLanguage CurrentLanguage { get; private set; } = SystemLanguage.Korean;

        private readonly Dictionary<SystemLanguage, Dictionary<string, string>> allLanguageTables = new();
        private Dictionary<string, string> curLanguageTable = new();

        public CDLocalizingSystem()
        {
            LoadAllLanguageTables();
        }

        public SystemLanguage ResolveLanguage(SystemLanguage deviceLang)
        {
            return HasLanguage(deviceLang) ? deviceLang : SystemLanguage.Korean;
        }

        public bool HasLanguage(SystemLanguage lang) => allLanguageTables.ContainsKey(lang);

        public void ApplyLanguage(SystemLanguage lang)
        {
            if (!HasLanguage(lang))
                throw new ArgumentException($"[Localization] Unsupported language: {lang}", nameof(lang));

            CurrentLanguage = lang;
            curLanguageTable = allLanguageTables[lang];
        }

        public string GetText(string key)
        {
            if (curLanguageTable == null)
                return $"[NO_TABLE::{key}]";

            if (!curLanguageTable.TryGetValue(key, out var value))
                return $"[MISSING_KEY::{key}]";

            return value;
        }

        private void LoadAllLanguageTables()
        {
            allLanguageTables[SystemLanguage.Korean] = new Dictionary<string, string>
            {
                { "UI_MAIN_START_BUTTON", "시작하기" },
                { "UI_SHOP_BUY",          "구매하기" },
                { "UI_ERROR_NETWORK",     "네트워크 오류" },
            };

            allLanguageTables[SystemLanguage.English] = new Dictionary<string, string>
            {
                { "UI_MAIN_START_BUTTON", "Start" },
                { "UI_SHOP_BUY",          "Buy" },
                { "UI_ERROR_NETWORK",     "Network Error" },
            };

            allLanguageTables[SystemLanguage.Japanese] = new Dictionary<string, string>
            {
                { "UI_MAIN_START_BUTTON", "スタート" },
                { "UI_SHOP_BUY",          "購入する" },
                { "UI_ERROR_NETWORK",     "ネットワークエラー" },
            };
        }
    }

    public class CDLocalizingManager : MonoSingleton<CDLocalizingManager>
    {
        public event Action<SystemLanguage> OnLanguageChanged;

        public SystemLanguage CurLanguage => localizingSystem.CurrentLanguage;

        private CDLocalizingSystem localizingSystem;

        protected override void Awake()
        {
            base.Awake();

            if (localizingSystem != null)
                return;

            localizingSystem = new CDLocalizingSystem();
            var initialLang = localizingSystem.ResolveLanguage(Application.systemLanguage);
            ApplyLanguageInternal(initialLang);
        }

        public void SetLanguage(SystemLanguage lang)
        {
            if (lang == CurLanguage)
                return;

            if (!localizingSystem.HasLanguage(lang))
            {
                Debug.LogWarning($"[Localization] {lang} 테이블이 없습니다.");
                return;
            }

            ApplyLanguageInternal(lang);
            OnLanguageChanged?.Invoke(CurLanguage);
        }

        public string GetText(string key)
        {
            return localizingSystem?.GetText(key) ?? $"[NO_SYSTEM::{key}]";
        }

        private void ApplyLanguageInternal(SystemLanguage lang)
        {
            localizingSystem.ApplyLanguage(lang);
            Debug.Log($"[Localization] Language Applied: {CurLanguage}");
        }
    }
}
