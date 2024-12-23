﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour, IMenu
{
    CraftStation currentCrafting;
    public Canvas canvas;
    public GameObject template;
    public Transform container;
    public Transform display;
    public Transform displayText;
    public Transform ingredientContain;
    public Transform craftButton;
    public GameObject intoAnim; 
    List<Transform> invDisplay = new List<Transform>();

    Item currentItem;
    Inventory plrInv;
    PlayerManager plr;

    void Start()
    {
        plrInv = FindObjectOfType<Inventory>();
        plr = FindObjectOfType<PlayerManager>();
        foreach (Transform display in ingredientContain)
        {
            invDisplay.Add(display);
        }
    }
    bool open = false;
    public void OpenMenu(CraftStation info)
    {
        if (!open && plr.SetCurrentUI(this))
        {
            currentCrafting = info;
            canvas.enabled = true;
            ClearIngredients();
            SetObjects();
            DisableButton();
            open = true;
        }
    }
    public void CloseMenu()
    {
        if (open && plr.SetCurrentUI(null))
        {
            canvas.enabled = false;
            currentItem = null;
            ClearObjects();
            ClearDisplay();
            open = false;
        }
    }
    public Canvas GetCanvas()
    {
        return canvas;
    }
    public void ResetSelection()
    {
        foreach (Transform child in container)
        {
            child.GetComponent<CraftItem>().Unselect();
        }
    }
    public void DisplayItem(Item item)
    {
        ClearDisplay();
        currentItem = item;
        Image imag = display.GetComponent<Image>();
        imag.sprite = item.asset;
        imag.color = new Color32(255, 255, 255, 255);
        if (item.amountToCraft > 1)
        {
            displayText.GetComponent<Text>().text = item.amountToCraft.ToString();
        }

        UpdateButton();
        int i = 0;
        foreach (ItemInfo info in item.materials)
        {
            if (i < 5 && info.item != null)
            {
                Image image = invDisplay[i].Find("Image").GetComponent<Image>();
                Text text = invDisplay[i].Find("Text").GetComponent<Text>();
                image.sprite = info.item.asset;
                image.color = new Color32(255, 255, 255, 255);
                text.text = info.amount.ToString();
                if (plrInv.CheckItem(info))
                {
                    text.color = new Color32(255, 255, 255, 255);
                }
                else
                {
                    text.color = new Color32(255, 0, 0, 255);
                }
                i++;
            }
        }
    }
    void ClearDisplay()
    {
        ClearIngredients();
        Image image = display.GetComponent<Image>();
        image.sprite = null;
        image.color = new Color32(255, 255, 255, 0);
        displayText.GetComponent<Text>().text = "";
    }
    public void Craft()
    {
        if (currentItem && plrInv.GiveItems(currentItem.materials))
        {
            plrInv.CollectItem(new ItemInfo { item = currentItem, amount = currentItem.amountToCraft });
            DisplayItem(currentItem);
            AnimateItemInto(currentItem);
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
    void UpdateButton()
    {
        if (plrInv.CheckItems(currentItem.materials))
        {
            EnableButton();
        }
        else
        {
            DisableButton();
        }
    }
    void EnableButton()
    {
        craftButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        craftButton.GetComponent<ButtonAnims>().Enable();
    }
    void DisableButton()
    {
        craftButton.GetComponent<Image>().color = new Color32(210, 210, 210, 255);
        craftButton.GetComponent<ButtonAnims>().Disable();
    }
    void ClearIngredients()
    {
        foreach (Transform display in invDisplay)
        {
            Image image = display.Find("Image").GetComponent<Image>();
            Text text = display.Find("Text").GetComponent<Text>();
            image.color = new Color32(255, 255, 255, 0);
            image.sprite = null;
            text.text = "";
        }
    }
    void ClearObjects()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
    public void SetObjects()
    {
        ClearObjects();
        foreach (Item item in currentCrafting.items)
        {
            CraftItem button = Instantiate(template, container).GetComponent<CraftItem>();
            button.SetItem(item, this);
        }
    }
}
