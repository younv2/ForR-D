/*
    CDGlobalLocalizingSystem v0.1

    작성 이유:
        - 프로젝트 내 모든 UI의 로컬라이징을 자동화하기 위해 작성.
        - 언어별 텍스트 테이블을 관리하고, 언어 변경 시 전체 UI에 알림을 전달.

    특징:
        - 기기 언어(Application.systemLanguage)를 기반으로 초기 언어 자동 설정.
        - OnLanguageChanged 이벤트를 통해 모든 CDText가 자동 갱신됨.
        - 언어별 텍스트 테이블을 Dictionary 구조로 관리 (빠른 조회 및 캐싱).
        - MonoSingleton 기반으로 어디서든 접근 가능.

    Todo:
        1. 키를 문자열로 받기 때문에, 오타 시 오류 발생 → Enum 자동 생성 시스템 또는 Validation 툴 필요.
        2. JSON 또는 CSV 파일로부터 로컬라이징 데이터 자동 로드 기능 추가 필요.
*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CD
{
    public class CDGlobalLocalizingSystem : MonoSingleton<CDGlobalLocalizingSystem>
    {
        public SystemLanguage CurLanguage { get; private set; } = SystemLanguage.Korean;

        public event Action<SystemLanguage> OnLanguageChanged;

        private Dictionary<string, string> curLanguageTable = new();

        private readonly Dictionary<SystemLanguage, Dictionary<string, string>> allLanguageTables = new();

        protected override void Awake()
        {
            base.Awake();

            LoadAllLanguageTables();

            SystemLanguage curLang = GetPossibleLanguage(Application.systemLanguage);

            ApplyLanguageInternal(curLang);

        }

        private SystemLanguage GetPossibleLanguage(SystemLanguage deviceLang)
        {
            var supported = new HashSet<SystemLanguage>
            {
                SystemLanguage.Korean,
                SystemLanguage.English,
                SystemLanguage.Japanese
            };

            if (supported.Contains(deviceLang))
                return deviceLang;

            return SystemLanguage.Korean;
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

        public void SetLanguage(SystemLanguage lang)
        {
            if (lang == CurLanguage)
                return;

            if (!allLanguageTables.ContainsKey(lang))
            {
                Debug.LogWarning($"[Localization] {lang} 테이블이 없습니다.");
                return;
            }

            ApplyLanguageInternal(lang);

            OnLanguageChanged?.Invoke(CurLanguage);
        }

        private void ApplyLanguageInternal(SystemLanguage lang)
        {
            CurLanguage = lang;
            curLanguageTable = allLanguageTables[lang];
            Debug.Log($"[Localization] Language Applied: {CurLanguage}");
        }

        public string GetText(string key)
        {
            if (curLanguageTable == null)
                return $"[NO_TABLE::{key}]";

            if (!curLanguageTable.TryGetValue(key, out var value))
                return $"[MISSING_KEY::{key}]";

            return value;
        }

    }
}
