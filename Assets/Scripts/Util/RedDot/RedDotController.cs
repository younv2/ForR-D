using System.Collections.Generic;
/*
    RedDotController v0.1

    작성 이유:
        - 여러 시스템(미션, 메일, 광고 등)에서 발생하는 ‘알림 이유’를 하나의 중앙 관리자에서 집계하여
          최종적으로 어떤 레드닷을 보여줄지 결정하기 위해 작성.
        - UI는 단순히 RedDot만 가지고 있고, "상태/이유 관리"는 컨트롤러가 전담하여
          UI 로직과 도메인 로직을 명확히 분리하기 위함.

    특징:
        - Slot 단위로 RedDot 오브젝트와 reasons(HashSet<ERedDotType>)를 관리.
          → 하나의 콘텐츠에 대해 **다수의 알림 사유**가 있을 수 있고, 이를 모두 유지해야 함.
        - Evaluate(content)는 reasons를 기반으로 **우선순위(Priority)**를 적용하여
          실제 UI에 표시할 최종 타입을 판단.
        - ShowOrHide(content, type, active) 하나로 사유 추가/삭제를 공통 처리하여 중복 제거.
        - Register()로 UI를 등록하고, 도메인 이벤트로 상태가 변화할 때는 Controller만 변경하면 되므로
          UI와 도메인의 의존성이 낮아짐.
        - 이벤트 기반 설계이므로 Update 없이 “상태 변화 시에만” UI가 갱신됨.

    Todo:
*/
public enum ERedDotContent
{
    Mail,
    MissionBtn
}
public enum ERedDotType
{
    Notification,
    Ads
}

public static class RedDotController
{
    private class Slot
    {
        public RedDot dot;
        public readonly HashSet<ERedDotType> reasons = new();
    }

    private static readonly Dictionary<ERedDotContent, Slot> map = new();

    private static readonly ERedDotType[] Priority =
    { 
        ERedDotType.Notification, 
        ERedDotType.Ads 
    };
    /// <summary>도메인 이벤트 구독</summary>
    public static void Install()
    {
    }

    /// <summary>HUD에서 RedDot을 등록</summary>
    public static void Register(RedDot dot)
    {
        if (dot == null) 
            return;
        ERedDotContent content = dot.Content;

        Slot slot = new Slot { dot = dot };
        map[content] = slot;

        dot.Init();
        Evaluate(content);
    }

    /// <summary>UI 파괴/비활성 시 호출 권장</summary>
    public static void Unregister(ERedDotContent content)
    {
        map.Remove(content);
    }

    private static void ShowOrHide(ERedDotContent content, ERedDotType type, bool active)
    {
        if (!map.TryGetValue(content, out var slot) || slot.dot == null) 
            return;

        if (active) 
            slot.reasons.Add(type);
        else 
            slot.reasons.Remove(type);

        Evaluate(content);
    }

    private static void Evaluate(ERedDotContent content)
    {
        if (!map.TryGetValue(content, out var slot) || slot.dot == null) 
            return;

        if (slot.reasons.Count == 0)
        {
            slot.dot.SetActive(default, false);
            return;
        }

        foreach (var type in Priority)
        {
            if (slot.reasons.Contains(type))
            {
                slot.dot.SetActive(type, true);
                return;
            }
        }

        foreach (var reason in slot.reasons)
        {
            slot.dot.SetActive(reason, true);
            return;
        }
    }

    #region 도메인 이벤트 핸들러
    //예시
    private static void OnMissionClaimableChanged(bool hasAny)
    {
        ShowOrHide(ERedDotContent.MissionBtn, ERedDotType.Notification, hasAny);
    }
    #endregion

}