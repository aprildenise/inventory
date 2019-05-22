using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{


    // References
    private Image backgroundImage;
    private Image itemImage;
    private RectTransform rt;

    // Variables
    public Item item; // change to private when done debugging
    private InventorySlot topLeftSlot;

    // Setups
    private void Awake()
    {

        // Get the image references
        // Get the background image
        backgroundImage = gameObject.GetComponent<Image>();
        if (backgroundImage == null)
        {
            Debug.LogWarning(gameObject.name + " is missing a backgroundImage.");
            return;
        }

        //Get the item image
        Transform child = gameObject.transform.GetChild(0);
        itemImage = child.GetComponent<Image>();

        // Get RectTransform
        rt = gameObject.GetComponent<RectTransform>();

        Debug.Log("Succesfully Init ItemUI");
    }


    public void SetItem(Item newItem)
    {
        item = newItem;
    }


    public string GetItemDescription()
    {
        if (item == null)
        {
            Debug.LogWarning(gameObject.name + " does not have an Item object in it.");
            return null;
        }
        else
        {
            return item.itemDescription;
        }
    }

    public void SetItemImage(Sprite image)
    {
        itemImage.sprite = image;
    }


    public void SetUISize(float width, float height)
    {
        //Debug.Log("sizeDelta:" + rt.sizeDelta);
        rt.sizeDelta = new Vector2(width, height);
    }


    public void SetPosition(float x, float y)
    {
        
        //Debug.Log("before:" + rt.anchoredPosition);
        rt.anchoredPosition = new Vector2(x, y * -1);
        //rt.pivot = new Vector2(0, 1);
        //Debug.Log("after:" + rt.anchoredPosition);
        
    }


    public void SetTopLeftSlot(InventorySlot newSlot)
    {
        topLeftSlot = newSlot;
    }


    public InventorySlot GetTopLeftSlot()
    {
        if (topLeftSlot == null)
        {
            Debug.LogWarning("The topLeftSlot of " + gameObject.name + " is not assigned.");
            return null;
        }
        else
        {
            return topLeftSlot;
        }
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
