using TMPro;
using UnityEngine;

// Keeps a drop-shadow copy of a heading showing the same words as the heading.
[RequireComponent(typeof(TMP_Text))]
public class CopyText : MonoBehaviour
{
    [SerializeField] TMP_Text shadow;
    TMP_Text source;

    public void Bind(TMP_Text target) => shadow = target;

    void Awake() => source = GetComponent<TMP_Text>();

    void LateUpdate()
    {
        if (shadow != null && shadow.text != source.text) shadow.text = source.text;
    }
}
