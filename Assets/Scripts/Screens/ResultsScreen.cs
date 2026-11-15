using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 9. Results: the stars, the coins, and three ways on (Menu, Retry, Next).
public class ResultsScreen : UIScreen
{
    [SerializeField] TMP_Text title, coinsText, nextLabel;
    [SerializeField] Image[] stars;
    [SerializeField] Color starOn, starOff;
    [SerializeField] GameObject bestBadge;
    [SerializeField] Transform[] buttons;

    int level, collected, total, starCount;
    bool newBest;

    public void Set(int level, int collected, int total, int stars)
    {
        this.level = level; this.collected = collected; this.total = total; starCount = stars;
        newBest = stars > GameData.GetStars(level);
        GameData.SetStars(level, stars);
        GameData.Coins += collected;
    }

    public override void OnEnter()
    {
        title.text = $"Level {level} complete!";
        nextLabel.text = level < GameData.LevelCount ? "Next" : "Levels";
        coinsText.text = "0";
        bestBadge.SetActive(false);
        // the stars land one by one after the card (OutBack, each with its own sound): a little drumroll
        for (int i = 0; i < stars.Length; i++)
        {
            bool on = i < starCount;
            stars[i].color = starOff;
            stars[i].transform.localScale = Vector3.one;
            if (!on) continue;
            var s = stars[i];
            Tween.Value(s, "color", 0.01f, Ease.Linear, t => { }, () =>
            {
                s.color = starOn;
                s.transform.localScale = Vector3.one * 0.3f;
                Tween.Scale(s.transform, Vector3.one, 0.4f, Ease.OutBack);
                Sfx.Star();
            }, 0.35f + i * 0.3f);
        }
        // the coins count up (OutCubic): quick at first, slow at the end so the total is readable
        Tween.Value(coinsText, "value", 0.8f, Ease.OutCubic, t => coinsText.text = $"+{Mathf.RoundToInt(collected * t)}  ({collected}/{total})", null, 0.3f);
        if (newBest)
        {
            Tween.Value(bestBadge, "value", 0.01f, Ease.Linear, t => { }, () =>
            {
                bestBadge.SetActive(true);
                bestBadge.transform.localScale = Vector3.zero;
                Tween.Scale(bestBadge.transform, Vector3.one, 0.4f, Ease.OutBack);
            }, 0.4f + starCount * 0.3f);
        }
        Stagger(buttons, 0.06f, 0.5f);
    }

    public void Menu() => UIManager.Instance.Home();

    public void Retry()
    {
        UIManager.Instance.Back();
        UIManager.Instance.Get<GameScreen>().Restart();
    }

    public void Next()
    {
        if (level >= GameData.LevelCount) { UIManager.Instance.BackTo(ScreenId.LevelSelect); return; }
        UIManager.Instance.Back();
        UIManager.Instance.Get<GameScreen>().StartLevel(level + 1);
    }
}
