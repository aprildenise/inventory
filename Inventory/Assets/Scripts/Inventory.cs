using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{

    // References
    [SerializeField] private GameObject UI;
    private List<InventoryPage> inventoryPages;
    [SerializeField] private TextMeshProUGUI descriptionBox;


    // Variables
    private List<Item> inventory;
    private int totalPages;
    private int totalItems;

    // Start is called before the first frame update
    private void Awake()
    {
        // Setups
        inventory = new List<Item>();
        totalItems = 0;
        totalPages = 1;

        // Get the correct references
        // UI reference
        if (UI == null)
        {
            Debug.LogWarning("Inventory Class is missing a reference to the inventory UI.");
        }

        // TMP reference 
        if (descriptionBox == null)
        {
            Debug.LogWarning("Inventory Class is missing a reference to the description box.");
            return;
        }


        // Find the list of Slots in the inventory UI
        // Get to the object that has the list of Pages
        Transform mainArea = UI.transform.Find("Main Area");
        if (mainArea == null)
        {
            Debug.LogWarning("Inventory Class cannot find the Main Area.", mainArea);
            return;
        }
        Transform slotsArea = mainArea.transform.Find("Slots Area");
        if (slotsArea == null)
        {
            Debug.LogWarning("Inventory Class cannot find the Slots Area.", slotsArea);
            return;
        }

        // Get all the Pages
        int numPages = slotsArea.childCount;
        inventoryPages = new List<InventoryPage>();
        for (int i = 0; i < numPages; i++)
        {
            // Loop through the slots area for the pages
            Transform child = slotsArea.GetChild(i);
            InventoryPage inventoryPageComp = child.GetComponent<InventoryPage>();
            if (inventoryPageComp == null)
            {
                Debug.LogWarning("Inventory Page object is missing an InventoryPage component.", child);
            }
            // Add the pages to the inventoryPages list
            inventoryPages.Add(inventoryPageComp);
        }

        // Make sure to hide the inventory when done.
        HideInventory();

        Debug.Log("Successfully Init Inventory");

    }

    public void DisplayInventory()
    {
        UI.SetActive(true);
    }

    public void HideInventory()
    {
        UI.SetActive(false);

    }


    public void AddNewItemToInventory(GameObject itemObject)
    {
        // Get the item property from the given item GameObject
        PickupItem pickUpItem = itemObject.GetComponent<PickupItem>();
        if (pickUpItem == null)
        {
            Debug.LogWarning("Item that was picked up does not have the PickupItem class on it.", pickUpItem);
            return;
        }
        Item item = pickUpItem.GetItemProperties();

        // Add the item to the inventory list
        inventory.Add(item);
        totalItems++;

        // Add the item to the inventory UI
        //Find where we can first place the item based on its dimensions
        int width = item.itemWidth;
        int height = item.itemHeight;

        //Check with page to use
        foreach (InventoryPage inventoryPage in inventoryPages)
        {
            InventorySlot[,] inventorySlots = inventoryPage.GetInventorySlots();
            int rows = inventorySlots.GetLength(0);
            int columns = inventorySlots.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // Check if we can place the item starting at this slot
                    if (CheckInventorySlot(i, j, inventorySlots, width, height))
                    {
                        // Place the item here
                        AddItemToSlot(inventorySlots, item, i, j, false);
                        return;
                    }
                }
            }
        }

        // No pages were found, create a new page
    }


    public void AddItemToSlot(InventorySlot[,] inventorySlots, Item item, int startRow, int startColumn, bool overrun)
    {
        // Add the item to the slot

        // Change the dimensions of the sprite
    }



    private bool CheckInventorySlot(int startRow, int startColumn, InventorySlot[,] inventorySlots, int width, int height)
    {

        // Find all the slots that will be occupied by the item
        // if it's top left index was at [row,column] (if 
        // it was placed here)
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                InventorySlot slot = inventorySlots[startRow + i, startColumn + j];
                if (slot.GetOccupancy() == true)
                {
                    // Item cannot be placed here
                    return false;
                }
            }
        }
        // Item can be placed here
        return true;
    }



}
