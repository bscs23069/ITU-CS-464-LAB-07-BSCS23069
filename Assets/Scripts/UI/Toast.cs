using TMPro;
using UnityEngine;

// A short message near the bottom ("Finish level 3 first"). It rises in with OutBack,
// stays long enough to read, then fades away, so feedback never needs a button to dismiss.
public class Toast : MonoBehaviour
{
    public static Toast Instance { get; private set; }

    [SerializeField] TMP_Text label;
    [SerializeField] CanvasGroup group;

    RectTransform rt;
    Vector2 rest;

    void Awake()
    {
        Instance = this;
        rt = (RectTransform)transform;
        rest = rt.anchoredPosition;
        group.alpha = 0f;
        group.blocksRaycasts = false;
    }

    public static void Show(string text)
    {
        if (Instance != null) Instance.Play(text);
    }

    void Play(string text)
    {
        label.text = text;
        rt.anchoredPosition = rest - new Vector2(0f, 60f);
        rt.localScale = Vector3.one * 0.9f;
        group.alpha = 0f;
        Tween.Move(rt, rest, 0.3f, Ease.OutBack);
        Tween.Scale(rt, Vector3.one, 0.3f, Ease.OutBack);
        Tween.Fade(group, 1f, 0.15f, Ease.OutQuad, () => Tween.Fade(group, 0f, 0.35f, Ease.InQuad, null, 1.6f));
    }
}
