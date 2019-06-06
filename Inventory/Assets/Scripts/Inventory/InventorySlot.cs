using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{

    // References
    private InventoryPage parentPage; // Inventory Page where this slot belongs to
    private ItemUI parentItemUI; // ItemUI that is current occupying this slot

    // Variables
    private bool isOccupied;
    private Vector2 index; // Index of this slot in the InventoryPage's inventorySlots grid
    //private Item item;

    private void Awake()
    {
        // Setups
        isOccupied = false;

        //Debug.Log("Successfully Init Inventory Slot");
    }


    public void SetIndex(int row, int col)
    { 
        index = new Vector2(col, row);
    }


    public Vector2 GetIndex()
    {
        return index;
    }


    public void SetParentItemUI(ItemUI ui)
    {
        parentItemUI = ui;
    }


    public ItemUI GetParentItemUI()
    {
        return parentItemUI;
    }


    public void SetParentPage(InventoryPage page)
    {
        parentPage = page;
    }


    public InventoryPage GetParentPage()
    {
        if (parentPage == null)
        {
            Debug.LogWarning(gameObject.name + " is missing a reference to the page it belongs to.");
        }
        return parentPage;
    }


    //public void SetItem(Item item)
    //{
    //    this.item = item;
    //    isOccupied = true;
    //}



    public bool GetOccupancy()
    {
        return isOccupied;
    }


    public void SetOccupancy(bool occupancy)
    {
        isOccupied = occupancy;
    }


}
