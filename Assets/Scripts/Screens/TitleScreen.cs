using UnityEngine;

// 1. Title: the game's name and a "tap anywhere" prompt. The whole screen is one button.
public class TitleScreen : UIScreen
{
    [SerializeField] RectTransform logo;
    [SerializeField] RectTransform hopper;
    [SerializeField] CanvasGroup tapText;

    Vector2 logoRest;

    void Awake() => logoRest = logo.anchoredPosition;

    public override void OnEnter()
    {
        // the logo drops in and bounces (OutBounce): playful, and it sets the game's tone in one second
        logo.anchoredPosition = logoRest + new Vector2(0f, 700f);
        Tween.Move(logo, logoRest, 0.9f, Ease.OutBounce);
        // then the hopper pops in under it (OutBack)
        hopper.localScale = Vector3.zero;
        Tween.Scale(hopper, Vector3.one, 0.45f, Ease.OutBack, null, 0.6f);
        // the prompt appears last, when there is something to start
        tapText.alpha = 0f;
        Tween.Fade(tapText, 1f, 0.4f, Ease.OutQuad, null, 1.0f);
    }

    public void TapToStart()
    {
        if (UIManager.Instance.Busy) return;
        UIManager.Instance.Replace(ScreenId.MainMenu);
    }
}
