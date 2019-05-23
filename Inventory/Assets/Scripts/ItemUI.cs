using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemUI : MonoBehaviour, ISelectHandler
{


    // References
    private Image backgroundImage; // May not be needed
    private Image itemImage;
    private RectTransform rt;
    public EventSystem eventSystem;

    public TextMeshProUGUI descriptionBox; // May not be needed
    public TextMeshProUGUI nameBox;

    // Variables
    private Item item; // change to private when done debugging
    private InventorySlot topLeftSlot;
    //private bool interactionsDisplayed;

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

        // Get interactions 
        //interactions = gameObject.transform.GetChild(1);

        Debug.Log("Succesfully Init ItemUI");
    }


    public void HoldItem()
    {
        // If the player is holding the item, follow references back to the
        // inventory page in case the player wants to move the UI to another
        // slot in the inventory.
        InventoryPage page = topLeftSlot.GetParentPage();
        page.MoveUI(gameObject.GetComponent<ItemUI>());

    }


    /// <summary>
    /// Set which item is in this item UI. Also make sure the UI shows the correct sprite.
    /// </summary>
    /// <param name="newItem"></param>
    public void SetItem(Item newItem)
    {
        item = newItem;
        itemImage.sprite = newItem.itemSprite;
    }



    /// <summary>
    /// Get the item description from the item represented by the ui
    /// </summary>
    /// <returns></returns>
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



    /// <summary>
    /// Set the overall size of this UI.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetUISize(float width, float height)
    {
        rt.sizeDelta = new Vector2(width, height);
    }



    /// <summary>
    /// Set the position of this UI.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetPosition(float x, float y)
    {
        
        rt.anchoredPosition = new Vector2(x, y * -1);
        
    }



    /// <summary>
    /// Set the top left slot, which is the slot that is in the top left corner of this UI.
    /// </summary>
    /// <param name="newSlot"></param>
    public void SetTopLeftSlot(InventorySlot newSlot)
    {
        topLeftSlot = newSlot;
    }



    /// <summary>
    /// Get the top left slot of this UI.
    /// </summary>
    /// <returns></returns>
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



    /// <summary>
    /// Use the event system and choose what is selected by it. Used for UI navigation
    /// </summary>
    /// <param name="selected"></param>
    public void SetSelectedObject(GameObject selected)
    {
        eventSystem.SetSelectedGameObject(selected);
    }


    /// <summary>
    /// Called whenever the event selects this gameObject
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log("descriptionbox:" + descriptionBox);
        //Debug.Log("item description:" + item.itemDescription);
        nameBox.text = item.itemName;
        descriptionBox.text = item.itemDescription;
    }
}
