/*
    CDText v0.2

    작성 이유:
        - 다국어 로컬라이징 적용을 자동화하기 위해 TextMeshProUGUI를 감싸는 나만의 텍스트 클래스 작성.
        - 각 UI 오브젝트마다 텍스트를 직접 수정하지 않아도, 언어 변경 시 자동으로 반영되도록 함.
        - 다국어에 따른 폰트 변경 정책도 함께 적용.
    특징:
        - TextMeshProUGUI를 직접 상속하지 않고 컴포넌트로 분리 (유지보수 및 확장성 향상)
        - OnEnable 시 자동으로 현재 언어로 동기화
        - CDLocalizingManager.OnLanguageChanged 이벤트를 통해 자동 업데이트
        - 다이얼로그나 실시간 텍스트 변경 UI의 경우 key가 동적으로 바뀌어야 함 → SetText를 통한 문자열 변경.

    Todo:
        1. key를 문자열로 입력받기 때문에, 오타 발생 시 치명적 → Enum 또는 ScriptableObject 기반 키 선택 구조로 개선 필요.
*/

using TMPro;
using UnityEngine;

namespace CD
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CDText : MonoBehaviour
    {
        public enum FontPolicy
        {
            Auto,
            Fixed
        }
        [SerializeField] private string m_Key;
        [SerializeField] private FontPolicy m_FontPolicy;

        private TextMeshProUGUI m_Text;
        private CDLocalizingManager m_LocalizingMgr;

        private bool m_IsIgnoreLocalization = false;

        private void Awake()
        {
            Init();
        }
        
        private void OnEnable()
        {
            Init();
            if (m_LocalizingMgr == null)
            {
                m_Text.text = $"[NO_MANAGER:{m_Key}]";
                return;
            }

            m_LocalizingMgr.OnLanguageChanged += HandleLanguageChanged;

            RefreshText();
        }
        private void OnDisable()
        {
            if (m_LocalizingMgr != null)
                m_LocalizingMgr.OnLanguageChanged -= HandleLanguageChanged;
        }
        private void Init()
        {
            if (m_LocalizingMgr != null)
                return;
            m_Text = GetComponent<TextMeshProUGUI>();
            m_LocalizingMgr = CDLocalizingManager.Instance;
        }
        /// <summary>
        /// 텍스트를 직접 세팅합니다. 로컬라이징 매니저의 영향을 받지 않습니다.
        /// </summary>
        /// <param name="_text"></param>
        public void SetText(string _text)
        {
            m_Text.text = _text;
            m_IsIgnoreLocalization = true;
        }
        /// <summary>
        /// 로컬라이즈 키를 세팅합니다.
        /// </summary>
        /// <param name="_key"></param>
        public void SetLocalizeKey(string _key)
        {
            m_Key = _key;
            m_IsIgnoreLocalization = false;
            RefreshText();
        }
        /// <summary>
        /// 텍스트를 로컬라이징 매니저에서 가져와 갱신합니다.
        /// </summary>
        private void RefreshText()
        {
            m_Text.text = m_LocalizingMgr.GetText(m_Key);
        }
        /// <summary>
        /// 국가별 폰트 정책에 따라 폰트를 적용합니다.
        /// </summary>
        /// <param name="_lang"></param>
        private void ApplyFont(SystemLanguage _lang)
        {
            if (m_FontPolicy == FontPolicy.Fixed)
                return;

            m_Text.font = m_LocalizingMgr.GetFontByLangauge(_lang);
        }

        /// <summary>
        /// 핸들러 : 언어 변경 시 호출됩니다.
        /// </summary>
        /// <param name="_lang"></param>
        private void HandleLanguageChanged(SystemLanguage _lang)
        {
            if (!m_IsIgnoreLocalization)
                RefreshText();

            ApplyFont(_lang);
        }
    }
}
