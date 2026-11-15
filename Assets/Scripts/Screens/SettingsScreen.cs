using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 6. Settings: two volume sliders, two switches, and a reset that asks twice.
public class SettingsScreen : UIScreen
{
    [SerializeField] Slider music, sound;
    [SerializeField] SwitchToggle vibration, reduceMotion;
    [SerializeField] Button resetButton;
    [SerializeField] TMP_Text resetLabel;
    [SerializeField] Image resetImage;
    [SerializeField] Color resetColor, confirmColor;
    [SerializeField] Transform[] groups;

    float confirmUntil = -1f;

    void Awake()
    {
        music.onValueChanged.AddListener(v => { GameData.Music = v; Sfx.Instance?.Apply(); });
        sound.onValueChanged.AddListener(v => { GameData.Sound = v; Sfx.Instance?.Apply(); });
        vibration.changed.AddListener(on => { GameData.Vibration = on; if (on) Sfx.Buzz(); });
        reduceMotion.changed.AddListener(on => { GameData.ReduceMotion = on; Tween.ReduceMotion = on; });
    }

    public override void OnEnter()
    {
        music.SetValueWithoutNotify(GameData.Music);
        sound.SetValueWithoutNotify(GameData.Sound);
        vibration.Set(GameData.Vibration, false);
        reduceMotion.Set(GameData.ReduceMotion, false);
        CancelReset();
        Stagger(groups, 0.07f, 0.12f);
    }

    void Update()
    {
        if (confirmUntil > 0f && Time.unscaledTime > confirmUntil) CancelReset();
    }

    void CancelReset()
    {
        confirmUntil = -1f;
        resetLabel.text = "Reset progress";
        resetImage.color = resetColor;
    }

    // a destructive button asks again: the first tap turns it red and shakes it, the second resets
    public void ResetProgress()
    {
        if (confirmUntil < 0f)
        {
            confirmUntil = Time.unscaledTime + 3f;
            resetLabel.text = "Tap again to reset";
            resetImage.color = confirmColor;
            Sfx.Error();
            Tween.Shake((RectTransform)resetButton.transform, 10f, 0.35f);
            return;
        }
        GameData.ResetAll();
        Sfx.Instance?.Apply();
        OnEnter();
        Toast.Show("Progress reset");
    }
}
