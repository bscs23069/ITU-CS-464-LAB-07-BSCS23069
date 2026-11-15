using UnityEngine;
using UnityEngine.EventSystems;

// 10. Credits: the names roll up slowly (Linear: an even reading speed, no easing on purpose).
// Holding a finger on the list stops the roll, so you can read at your own pace.
public class CreditsScreen : UIScreen, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] RectTransform content;
    [SerializeField] RectTransform viewport;
    [SerializeField] float speed = 70f;

    float y;
    bool held;

    public override void OnEnter()
    {
        y = -viewport.rect.height * 0.25f;
        held = false;
        Apply();
    }

    void Update()
    {
        if (held) return;
        y += speed * Time.unscaledDeltaTime;
        if (y > content.rect.height) y = -viewport.rect.height;   // round again from the bottom
        Apply();
    }

    void Apply() => content.anchoredPosition = new Vector2(0f, y);

    public void OnPointerDown(PointerEventData e) => held = true;
    public void OnPointerUp(PointerEventData e) => held = false;
}
