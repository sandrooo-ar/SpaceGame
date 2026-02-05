using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestPannel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI coins;
    public GameObject[] items;

    public int currentItemsIndex = 0;

    public TextMeshProUGUI rerollText;

    public int rerollPrice = 50;

    public Button rerollButton;

    private void OnEnable()
    {
        ChangeCoins();
        ReactivateItems();
        currentItemsIndex = items.Length;
    }

    public void ItemBought()
    {
        currentItemsIndex--;
        if (currentItemsIndex <= 0)
        {
            StartCoroutine(AllItemsBought());
        }
    }

    private IEnumerator AllItemsBought()
    {
        rerollButton.interactable = false;

        yield return new WaitForSecondsRealtime(1f);

        foreach (GameObject item in items)
        {
            item.GetComponent<UpgradeSlot>().ResetSlot();
            item.SetActive(false);
            item.SetActive(true);
            item.GetComponent<UpgradeSlot>().ApplyDiscount(item.GetComponent<UpgradeSlot>().discountPercentage);
        }

        currentItemsIndex = items.Length;

        rerollButton.interactable = true;

    }

    public void ChangeCoins()
    {
        coins.text = CoinManager.Instance.GetCoinCount().ToString();
        rerollText.text = rerollPrice.ToString();
    }

    public void RerollItems()
    {
        if (CoinManager.Instance.GetCoinCount() < rerollPrice)
        {
            return;
        }
        
        CoinManager.Instance.SpendCoins(rerollPrice);
        
        foreach (GameObject item in items)
        {
            if (!item.GetComponent<UpgradeSlot>().alreadyBought)
            {
                item.GetComponent<UpgradeSlot>().ResetSlot();
                item.SetActive(false);
                item.SetActive(true);
                item.GetComponent<UpgradeSlot>().ApplyDiscount(item.GetComponent<UpgradeSlot>().discountPercentage);
            }
        }

        rerollPrice *= 2;
        ChangeCoins();
    }

    private void ReactivateItems()
    {
        foreach (GameObject item in items)
        {
            if (!item.activeSelf)
            {
                item.SetActive(true);
            }
        }
    }

}
