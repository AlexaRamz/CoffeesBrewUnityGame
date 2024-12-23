using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour, IMenu
{
    public Shop shopInfo;
    public Canvas canvas;
    Inventory plrInv;
    PlayerManager plr;
    public Transform container;
    public GameObject template;
    public Animator coinAnim;
    public GameObject intoAnim;
    public Text moneyDisplay;
    public Text dialogueDisplay;

    void Start()
    {
        plrInv = FindObjectOfType<Inventory>();
        plr = FindObjectOfType<PlayerManager>();
    }
    public void Buy(Item item)
    {
        Debug.Log(item.price);
        plrInv.Spend(item.price, new ItemInfo { item = item, amount = 1 });
        AnimateMoneyDisplay();
        UpdateMoneyDisplay();
        UpdateColors();
        AnimateItemInto(item);
    }
    bool open = false;
    public void OpenMenu(Shop info)
    {
        if (!open && plr.SetCurrentUI(this))
        {
            shopInfo = info;
            UpdateMoneyDisplay();
            canvas.enabled = true;
            plrInv.HideInterface();
            SetObjects();
            open = true;
        }
    }
    public void CloseMenu()
    {
        if (open && plr.SetCurrentUI(null))
        {
            shopInfo = null;
            canvas.enabled = false;
            plrInv.ShowInterface();
            ClearObjects();
            open = false;
        }
    }
    public Canvas GetCanvas()
    {
        return canvas;
    }
    void AnimateMoneyDisplay()
    {
        coinAnim.ResetTrigger("Pop");
        coinAnim.SetTrigger("Pop");
    }
    void UpdateMoneyDisplay()
    {
        moneyDisplay.text = plrInv.money.ToString();
    }
    public void UpdateDescription(Item item)
    {
        string text = item.name + '\n' + item.description;
        dialogueDisplay.text = text;
    }
    public void HideDescription()
    {
        dialogueDisplay.text = "Shop for farm supplies here.";
        ShowOptions();
    }
    void ShowOptions()
    {

    }
    void UpdateColors()
    {
        foreach (Transform button in container)
        {
            button.GetComponent<ShopItem>().UpdateColor(plrInv.money);
        }
    }
    void ClearObjects()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
    void AnimateItemInto(Item item)
    {
        Animator anim = intoAnim.GetComponent<Animator>();
        Image image = intoAnim.GetComponent<Image>();
        image.sprite = item.asset;
        image.color = new Color32(255, 255, 255, 255);
        anim.ResetTrigger("Into");
        anim.SetTrigger("Into");
    }
    void SetObjects()
    {
        ClearObjects();
        foreach (Item item in shopInfo.items)
        {
            ShopItem button = Instantiate(template, container).GetComponent<ShopItem>();
            button.SetItem(item, this);
            button.UpdateColor(plrInv.money);
        }
    }
}
