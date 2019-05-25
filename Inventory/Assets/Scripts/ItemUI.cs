using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ItemUI : MonoBehaviour, ISelectHandler
{


    // References
    private Image backgroundImage; // May not be needed
    private Image itemImage;
    private RectTransform rt;
    public EventSystem eventSystem;
    public InventoryTextHandler inventoryText;

    // Variables
    private Item item; // change to private when done debugging
    private InventorySlot topLeftSlot;
    //private bool interactionsDisplayed;
    //private bool isHolding;

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
        // isHolding = true;


        //InventoryPage page = topLeftSlot.GetParentPage();
        //page.SetUIIsMiving(gameObject.GetComponent<ItemUI>());

    }


    public void PlaceItemUI()
    {
        Debug.Log("held interaction button is pressed....!");
        // Get a reference to the inventory page in order to see if we can place the ui here
        InventoryPage page = topLeftSlot.GetParentPage();

        bool isPlaced = page.PlaceItemUI(gameObject.gameObject.GetComponent<ItemUI>(), GetPosition());
        if (isPlaced)
        {
            // Successfully placed. Turn off the itemDragHandler and its button and reset
            // what is selected by the event system
            eventSystem.SetSelectedGameObject(this.gameObject);

            //Turn off the shadow for effect
            Shadow shadow = gameObject.GetComponent<Shadow>();
            shadow.enabled = false;
        }
        else
        {
            // Item cannot be placed here. Do not turn off the itemDragHandler yet. 
            // Display that the item cannot be placed at this spot
        }
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


    public Item GetItem()
    {
        return item;
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

        rt.anchoredPosition = new Vector2(x, y);
        
    }


    public Vector2 GetPosition()
    {
        return rt.anchoredPosition;
    }



    public void MoveUI(Vector2 moveInput)
    {

        float x = moveInput.x * 60;
        float y = moveInput.y * 60;

        Vector2 shift = new Vector2(x, y);
        Vector2 currentPos = GetPosition();

        Vector2 newPos = currentPos + shift;
        Debug.Log("currentpos:" + currentPos);
        Debug.Log("newpos:" + newPos);

        // Make sure it doesn't go over the page
        // Prevent the UI from moving too far
        if (newPos.x < 20f || newPos.x > 440f)
        {
            // don't shift the x position
            newPos.x = currentPos.x;
        }
        if (newPos.y > -2f || newPos.y < -500f)
        {
            // don't shift the y position
            newPos.y = currentPos.y;
        }

        // Set new position
        SetPosition(newPos.x, newPos.y);

        // Shift to a new page if needed
    }


    //public void ShiftPosition(float x, float y)
    //{
    //    Vector2 shift = new Vector2(x, y);
    //    Vector2 newPos = rt.anchoredPosition + shift;

    //    // Prevent the UI from moving too far
    //    if (newPos.x < 20f || newPos.x > 440f)
    //    {
    //        // don't shift the x position
    //        newPos.x = rt.anchoredPosition.x;
    //    }
    //    if (newPos.y > -2f || newPos.y < -500f)
    //    {
    //        // don't shift the y position
    //        newPos.y = rt.anchoredPosition.y;
    //    }

    //    // Assign the new postition
    //    rt.anchoredPosition = newPos;

    //}



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
        inventoryText.SetItemName(item.itemName);
        inventoryText.SetItemDescription(item.itemDescription);
    }

}
