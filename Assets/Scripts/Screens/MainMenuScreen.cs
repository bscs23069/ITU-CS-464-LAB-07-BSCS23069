using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 2. Main menu: the hub. One big Play button in the thumb zone, and the other screens one tap away.
public class MainMenuScreen : UIScreen
{
    [SerializeField] Image hopperBody;
    [SerializeField] TMP_Text hopperName;
    [SerializeField] Transform[] buttons;   // in the order they pop in: Play first

    public override void OnEnter()
    {
        ShowHopper();
        Stagger(buttons, 0.06f, 0.15f);
    }

    public override void OnResume() => ShowHopper();

    void ShowHopper()
    {
        var h = GameData.Hoppers[GameData.Selected];
        hopperBody.color = h.color;
        hopperName.text = h.name;
    }

    public void Play() => UIManager.Instance.Push(ScreenId.LevelSelect);
    public void Characters() => UIManager.Instance.Push(ScreenId.Characters);
    public void Shop() => UIManager.Instance.Push(ScreenId.Shop);
    public void Settings() => UIManager.Instance.Push(ScreenId.Settings);
    public void Credits() => UIManager.Instance.Push(ScreenId.Credits);
}
