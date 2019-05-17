using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{

    private Image itemImage;
    private Image backgroundImage;
    private bool isOccupied;
    private Item item;

    private void Start()
    {

        // Get background image
        Transform c = this.gameObject.transform.GetChild(0);
        if (c == null)
        {
            Debug.LogWarning("InventorySlot is missing a Background Image child.");
            return;
        }
        backgroundImage = c.gameObject.GetComponent<Image>();
        if (backgroundImage == null)
        {
            Debug.LogWarning("Background image component is missing from an Inventory Slot.");
            return;
        }

        // Get item image in child
        Transform child = this.gameObject.transform.GetChild(1);
        if (child == null)
        {
            Debug.LogWarning("InventorySlot is missing an Item Image child.");
            return;
        }
        itemImage = child.gameObject.GetComponent<Image>();
        if (itemImage == null)
        {
            Debug.LogWarning("Item Image Component is missing from an Inventory Slot.");
            return;
        }

        isOccupied = false;

        Debug.Log("Successfully Init Inventory Slot");
    }


    public void SetItem(Item item)
    {
        this.item = item;
        isOccupied = true;
    }

    public void UpdateImageDimensions(float width, float height)
    {
        
    }


    public bool GetOccupancy()
    {
        return isOccupied;
    }


    public void SetOccupancy(bool occupancy)
    {
        isOccupied = occupancy;
    }


}
