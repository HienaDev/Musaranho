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
        // Get the mouse position
        Vector2 mousePosition = Input.mousePosition;
        
        // Convert mouse position to canvas space if using Screen Space - Camera or World Space
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
            // For Screen Space - Overlay, we can use the mouse position directly
            cursorRectTransform.position = mousePosition;
        }
    }
}
