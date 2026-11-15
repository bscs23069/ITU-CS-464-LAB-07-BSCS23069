using TMPro;
using UnityEngine;
using UnityEngine.UI;

// The coin or star counter in the top bar. When the number changes it counts to the new value
// (OutCubic: fast first, then it slows so you can read the result) and bumps once.
public class CoinPill : MonoBehaviour
{
    public enum Kind { Coins, Stars }
    public Kind kind;
    [SerializeField] TMP_Text label;
    [SerializeField] Image background;

    float shown;
    Color normal;

    int Value => kind == Kind.Coins ? GameData.Coins : GameData.TotalStars;

    void Awake() => normal = background.color;

    void OnEnable()
    {
        GameData.Changed += Refresh;
        shown = Value;
        label.text = Mathf.RoundToInt(shown).ToString();
        background.color = normal;
    }

    void OnDisable() => GameData.Changed -= Refresh;

    void Refresh()
    {
        float from = shown, to = Value;
        if (Mathf.Approximately(from, to)) return;
        Tween.Value(label, "value", 0.5f, Ease.OutCubic, t =>
        {
            shown = Mathf.Lerp(from, to, t);
            label.text = Mathf.RoundToInt(shown).ToString();
        });
        Tween.PunchScale(transform, 0.18f, 0.35f);
    }

    // "not enough": the pill flashes red and shakes
    public void Deny()
    {
        var red = new Color(1f, 0.37f, 0.36f);
        Tween.Value(background, "color", 0.6f, Ease.OutQuad, t => background.color = Color.Lerp(red, normal, t));
        Tween.Shake((RectTransform)transform, 14f, 0.4f);
    }
}
