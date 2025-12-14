using System.Collections.Generic;
using UnityEngine;

namespace CD
{
    public static class CDLocalizingSystem
    {
        public static Dictionary<SystemLanguage, Dictionary<string, string>> BuildLanguageTables()
        {
            return new Dictionary<SystemLanguage, Dictionary<string, string>>
            {
                {
                    SystemLanguage.Korean, new Dictionary<string, string>
                    {
                        { "UI_MAIN_START_BUTTON", "시작하기" },
                        { "UI_SHOP_BUY",          "구매하기" },
                        { "UI_ERROR_NETWORK",     "네트워크 오류" },
                    }
                },
                {
                    SystemLanguage.English, new Dictionary<string, string>
                    {
                        { "UI_MAIN_START_BUTTON", "Start" },
                        { "UI_SHOP_BUY",          "Buy" },
                        { "UI_ERROR_NETWORK",     "Network Error" },
                    }
                },
                {
                    SystemLanguage.Japanese, new Dictionary<string, string>
                    {
                        { "UI_MAIN_START_BUTTON", "スタート" },
                        { "UI_SHOP_BUY",          "購入する" },
                        { "UI_ERROR_NETWORK",     "ネットワークエラー" },
                    }
                }
            };
        }

        public static SystemLanguage ResolveLanguage(SystemLanguage deviceLang, IReadOnlyDictionary<SystemLanguage, Dictionary<string, string>> tables, SystemLanguage fallback = SystemLanguage.Korean)
        {
            if (tables != null && tables.ContainsKey(deviceLang))
                return deviceLang;

            return fallback;
        }

        public static bool TryGetText(IReadOnlyDictionary<SystemLanguage, Dictionary<string, string>> tables, SystemLanguage lang, string key, out string value)
        {
            value = null;

            if (tables == null)
                return false;

            if (!tables.TryGetValue(lang, out var table) || table == null)
                return false;

            return table.TryGetValue(key, out value);
        }
    }
}
