using TMPro;
using UnityEngine;
using UnityEngine.UI;

// One thing for sale: a hopper, its price on the buy button, and an "Owned" tick once bought.
public class ShopRow : MonoBehaviour
{
    public int hopper;
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText, priceText;
    [SerializeField] Button buyButton;
    [SerializeField] GameObject ownedBadge;

    public RectTransform BuyRect => (RectTransform)buyButton.transform;
    public Transform Badge => ownedBadge.transform;

    public void Refresh()
    {
        var h = GameData.Hoppers[hopper];
        icon.color = h.color;
        nameText.text = h.name;
        priceText.text = h.price.ToString();
        bool owned = GameData.Owned(hopper);
        buyButton.gameObject.SetActive(!owned);
        ownedBadge.SetActive(owned);
    }
}
