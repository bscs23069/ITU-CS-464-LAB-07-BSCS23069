using TMPro;
using UnityEngine;
using UnityEngine.UI;

// One tile on the level grid: its number and stars when open, a lock when not.
public class LevelCell : MonoBehaviour
{
    public int level;
    [SerializeField] Image background;
    [SerializeField] TMP_Text number;
    [SerializeField] Image[] stars;
    [SerializeField] GameObject lockIcon;
    [SerializeField] Color openColor, lockedColor, starOn, starOff;

    public bool Open { get; private set; }

    public void Refresh()
    {
        Open = GameData.Unlocked(level);
        int got = GameData.GetStars(level);
        background.color = Open ? openColor : lockedColor;
        number.gameObject.SetActive(Open);
        lockIcon.SetActive(!Open);
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(Open);
            stars[i].color = i < got ? starOn : starOff;
        }
    }
}
