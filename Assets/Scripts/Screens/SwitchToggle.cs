using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// An on/off switch: the knob slides across (InOutQuad, it starts and stops at a rest position)
// and the track turns from grey to teal, so the state reads from position and colour together.
public class SwitchToggle : MonoBehaviour
{
    [SerializeField] RectTransform knob;
    [SerializeField] Image track;
    [SerializeField] Color onColor, offColor;
    [SerializeField] float travel = 52f;

    public UnityEvent<bool> changed = new UnityEvent<bool>();
    public bool On { get; private set; }

    public void Set(bool on, bool animate)
    {
        On = on;
        var to = new Vector2(on ? travel : -travel, 0f);
        Color from = track.color, target = on ? onColor : offColor;
        if (!animate)
        {
            knob.anchoredPosition = to;
            track.color = target;
            return;
        }
        Tween.Move(knob, to, 0.22f, Ease.InOutQuad);
        Tween.Value(track, "color", 0.22f, Ease.InOutQuad, t => track.color = Color.Lerp(from, target, t));
    }

    public void Flip()
    {
        Set(!On, true);
        changed.Invoke(On);
    }
}
