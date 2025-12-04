using TMPro;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace CD.EditorTools
{
    public static class CDMenuItems
    {
        [MenuItem("GameObject/CD/Text", false, 10)]
        public static void CreateCDText(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Text", typeof(RectTransform),typeof(CDText));

            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            var tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.text = "New Text";
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 50);

            var cdText = go.GetComponent<CDText>();
            ComponentUtility.MoveComponentUp(cdText);

            Selection.activeObject = go;

            Undo.RegisterCreatedObjectUndo(go, "Create Text");
        }
        /*
        [MenuItem("GameObject/CD/CDButton", false, 11)]
        public static void CreateCDButton(MenuCommand menuCommand)
        {
            // 버튼 오브젝트 생성
            GameObject go = new GameObject("CDButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(CD.CDButton));

            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // 자식으로 CDText 생성
            var textGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(CD.CDText));
            textGO.transform.SetParent(go.transform, false);

            // 기본 버튼 세팅
            var img = go.GetComponent<Image>();
            img.color = new Color(0.2f, 0.5f, 1f, 0.5f);

            var btn = go.GetComponent<Button>();
            btn.targetGraphic = img;

            // Label Text 설정
            var tmp = textGO.GetComponent<TextMeshProUGUI>();
            tmp.text = "New CDButton";
            tmp.alignment = TextAlignmentOptions.Center;

            // RectTransform 기본 세팅
            var rectButton = go.GetComponent<RectTransform>();
            rectButton.sizeDelta = new Vector2(160, 40);

            var rectText = textGO.GetComponent<RectTransform>();
            rectText.anchorMin = Vector2.zero;
            rectText.anchorMax = Vector2.one;
            rectText.offsetMin = Vector2.zero;
            rectText.offsetMax = Vector2.zero;

            Selection.activeObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create CDButton");
        }
        */
    }
}