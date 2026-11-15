using UnityEngine;

// 8. Pause: a card over the frozen game. Resume is the biggest button, Quit the quietest.
public class PauseScreen : UIScreen
{
    [SerializeField] Transform[] buttons;

    public override void OnEnter() => Stagger(buttons, 0.05f, 0.12f);

    public void Resume() => UIManager.Instance.Back();

    public void Restart()
    {
        UIManager.Instance.Back();
        UIManager.Instance.Get<GameScreen>().Restart();
    }

    public void Settings() => UIManager.Instance.Push(ScreenId.Settings);
    public void Quit() => UIManager.Instance.Home();
}
