using UnityEngine;
using UnityEngine.UI;
/*
    RedDot v0.1

    작성 이유:
        - UI 버튼(HUD 버튼 등)에 표시되는 "레드닷(알림 표시)"을 통일된 방식으로 처리하기 위한 전용 컴포넌트.
        - GameObject 활성/비활성 + 스프라이트 전환을 단순화하고,
          외부 시스템(RedDotController)에서 타입 기반으로 쉽게 조작할 수 있게 하기 위함.
        - 각 버튼은 하나의 RedDot만 가지며, 자신의 콘텐츠(ERedDotContent)를 알고 있어
          Register 시 자동으로 컨트롤러에 매핑되도록 함.

    특징:
        - Image 컴포넌트를 기반으로 동작하며, Init()에서 안전하게 레퍼런스를 확보.
        - SetActive(type, flag)를 통해 "타입 전환 + 표시/숨김"을 한 번에 처리.
        - 스프라이트 변경(SetType)은 레드닷의 시각적 표현만 담당하며, 로직은 전부 Controller에서 관리.
        - 내부적으로 타입/스프라이트 외에는 상태를 저장하지 않음(상태는 Controller가 관리).

    Todo:
        1. 레드닷 애니메이션(펄스, 팝업) 추가 시 Animator 또는 DOTween 연동 고려.
*/
public class RedDot : MonoBehaviour
{
    [SerializeField] private ERedDotContent content;

    public ERedDotContent Content { get { return content; } }

    private Image dotImage;

    public void Init()
    {
        InitComponent();

        gameObject.SetActive(false);
    }
    private void InitComponent()
    {
        if (!dotImage)
            dotImage = GetComponent<Image>();
    }
    public void SetActive(ERedDotType _type, bool _flag)
    {
        Init();

        if (!_flag)
            return;

        SetType(_type);
        gameObject.SetActive(true);
    }
    public void SetType(ERedDotType _type)
    {
        switch (_type)
        {
            case ERedDotType.Notification:
                //m_Image.sprite = ;
                break;
            case ERedDotType.Ads:
                //m_Image.sprite = ;
                break;
            default:
                //m_Image.sprite = ;
                break;
        }
    }
}
