/*
    CDLocalizingManager v0.2

    작성 이유:
        - 순수 로직(System)과 Unity 진입점(Manager)을 분리해 테스트/확장이 용이하도록 개선.
*/
using System;
using System.Collections.Generic;
using TMPro;
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
        public string GetText(string _key, params object[] _params)
        {
            if (curLanguageTable == null)
                return $"[NO_TABLE::{_key}]";
            var text = curLanguageTable.TryGetValue(_key, out var value) ? value : $"[MISSING_KEY::{_key}]";
            text = string.Format(text, _params);
            return text;
        }

        private void ApplyLanguageInternal(SystemLanguage _lang)
        {
            CurLanguage = _lang;
            curLanguageTable = languageTables[_lang];
            Debug.Log($"[Localization] Language Applied: {CurLanguage}");
        }
        //TODO : 폰트 캐시 처리 필요 및 어드레서블 고려
        public TMP_FontAsset GetFontByLangauge(SystemLanguage _curLang)
        {
            switch(_curLang)
            {
                case SystemLanguage.Korean:
                    return Resources.Load<TMP_FontAsset>("Fonts/NotoSansKR-Medium SDF");
                case SystemLanguage.Japanese:
                    return Resources.Load<TMP_FontAsset>("Fonts/NotoSansJP-Medium SDF");
                case SystemLanguage.English:
                default:
                    return Resources.Load<TMP_FontAsset>("Fonts/Roboto-Medium SDF");
            }
        }
    }
}
