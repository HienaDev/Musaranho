using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private RectTransform cursorRectTransform;

    private void Update()
    {
        UpdateCursorPosition();
    }

    private void UpdateCursorPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        
        if (parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                mousePosition,
                parentCanvas.worldCamera,
                out Vector2 localPoint);

            cursorRectTransform.localPosition = localPoint;
        }
        else
        {
            cursorRectTransform.position = mousePosition;
        }
    }
}
