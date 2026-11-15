using UnityEngine;

// A slow idle float (InOutSine both ways) for the hopper on the menus and the "tap to start" text.
// It runs in unscaled time because the menus stop the game clock.
public class Bob : MonoBehaviour
{
    public float height = 18f;
    public float period = 2.2f;
    public float pulse = 0f;     // scale breathing, for the "tap to start" text
    public float phase = 0f;

    RectTransform rt;
    Vector2 rest;

    void Awake()
    {
        rt = (RectTransform)transform;
        rest = rt.anchoredPosition;
    }

    void Update()
    {
        if (Tween.ReduceMotion) { rt.anchoredPosition = rest; return; }
        float t = Mathf.PingPong(Time.unscaledTime / (period * 0.5f) + phase, 1f);
        float e = Ease.InOutSine(t);
        rt.anchoredPosition = rest + new Vector2(0f, height * e);
        if (pulse > 0f) rt.localScale = Vector3.one * (1f + pulse * e);
    }
}
