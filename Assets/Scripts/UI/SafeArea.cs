using UnityEngine;

// Keeps the buttons and text inside the phone's safe area (away from the notch and the home bar).
// The backgrounds sit outside this object so they still fill the whole screen.
[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    Rect applied;
    Vector2Int size;

    void Update()
    {
        if (Screen.safeArea == applied && size.x == Screen.width && size.y == Screen.height) return;
        applied = Screen.safeArea;
        size = new Vector2Int(Screen.width, Screen.height);
        if (Screen.width <= 0 || Screen.height <= 0) return;
        var rt = (RectTransform)transform;
        rt.anchorMin = new Vector2(applied.xMin / Screen.width, applied.yMin / Screen.height);
        rt.anchorMax = new Vector2(applied.xMax / Screen.width, applied.yMax / Screen.height);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}
