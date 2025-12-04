/*
    CDText v0.1

    작성 이유:
        - 다국어 로컬라이징 적용을 자동화하기 위해 TextMeshProUGUI를 감싸는 나만의 텍스트 클래스 작성.
        - 각 UI 오브젝트마다 텍스트를 직접 수정하지 않아도, 언어 변경 시 자동으로 반영되도록 함.

    특징:
        - TextMeshProUGUI를 직접 상속하지 않고 컴포넌트로 분리 (유지보수 및 확장성 향상)
        - OnEnable 시 자동으로 현재 언어로 동기화
        - CDGlobalLocalizingManager.OnLanguageChanged 이벤트를 통해 자동 업데이트

    Todo:
        1. key를 문자열로 입력받기 때문에, 오타 발생 시 치명적 → Enum 또는 ScriptableObject 기반 키 선택 구조로 개선 필요.
        2. 다이얼로그나 실시간 텍스트 변경 UI의 경우 key가 동적으로 바뀌어야 함 → SetKey(string key) 메서드 추가 고려.
        3. 다국어에 따른 폰트/줄바꿈 스타일 조정 기능 추가 검토 (ex. 일본어/중국어 전용 폰트).
*/

using TMPro;
using UnityEngine;

namespace CD
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CDText : MonoBehaviour
    {
        //인스펙터 창에서 키를 입력
        [SerializeField] private string key;

        private TextMeshProUGUI text;
        public string Text => text.text;

        private CDGlobalLocalizingSystem localizingMgr;

        private void Awake()
        {
            Init();
            text = GetComponent<TextMeshProUGUI>();
        }
        private void Init()
        {
            if (localizingMgr != null)
                return;

            localizingMgr = CDGlobalLocalizingSystem.Instance;
        }
        private void OnEnable()
        {
            Init();
            if (localizingMgr == null)
            {
                text.text = $"[NO_MANAGER:{key}]";
                return;
            }

            localizingMgr.OnLanguageChanged += HandleLanguageChanged;

            RefreshText();
        }
        private void OnDisable()
        {
            if (localizingMgr != null)
                localizingMgr.OnLanguageChanged -= HandleLanguageChanged;
        }
        
        private void HandleLanguageChanged(SystemLanguage lang)
        {
            RefreshText();
        }

        private void RefreshText()
        {
            text.text = localizingMgr.GetText(key);
        }
    }

}
