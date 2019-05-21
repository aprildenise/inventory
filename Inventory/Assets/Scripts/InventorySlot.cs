using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{

    // References
    private InventoryPage parent;

    // Variables
    private bool isOccupied;
    private Item item;

    private void Start()
    {
        // Setups
        isOccupied = false;

        Debug.Log("Successfully Init Inventory Slot");
    }


    public void SetParentPage(InventoryPage page)
    {
        parent = page;
    }


    public InventoryPage GetParentPage()
    {
        if (parent == null)
        {
            Debug.LogWarning(gameObject.name + " is missing a reference to the page it belongs to.");
        }
        return parent;
    }


    public void SetItem(Item item)
    {
        this.item = item;
        isOccupied = true;
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
