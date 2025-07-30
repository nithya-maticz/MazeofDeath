using UnityEngine;

public class KeyBoard : MonoBehaviour
{
    public RectTransform panelToMove; // The UI panel or ScrollView content
    private float originalY;
    private bool isKeyboardVisible;

    void Start()
    {
        if (panelToMove != null)
            originalY = panelToMove.anchoredPosition.y;
    }

    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (TouchScreenKeyboard.visible)
        {
            Rect keyboardRect = TouchScreenKeyboard.area;

            if (keyboardRect.height > 0 && !isKeyboardVisible)
            {
                // Move panel up
                float shiftAmount = keyboardRect.height / Screen.dpi * 160f; // Convert to Unity units
                panelToMove.anchoredPosition = new Vector2(panelToMove.anchoredPosition.x, originalY + shiftAmount);
                isKeyboardVisible = true;
            }
        }
        else if (isKeyboardVisible)
        {
            // Reset panel position
            panelToMove.anchoredPosition = new Vector2(panelToMove.anchoredPosition.x, originalY);
            isKeyboardVisible = false;
        }
#endif
    }
}