using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{

    // References
    [SerializeField] private GameObject UI;
    private List<InventoryPage> inventoryPages;
    [SerializeField] private TextMeshProUGUI descriptionBox;
    [SerializeField] private EventSystem eventSystem;

    private InventoryPage currentPage;


    // Variables
    private List<Item> inventory;
    private int totalItems;
    private bool isUIActive;



    private void Awake()
    {
        // Setups
        inventory = new List<Item>();
        totalItems = 0;

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


        // EventSystem reference
        if (eventSystem == null)
        {
            Debug.LogWarning("Inventory Class is missing a reference to the Event System that manages it.");
            return;
        }


        // Find the list of Slots in the inventory UI
        // Get to the object that has the list of Pages
        Transform menu = UI.transform.Find("Menu");
        if (menu == null)
        {
            Debug.LogWarning("Inventory Class cannot find the Main Area.", menu);
            return;
        }
        Transform inven = menu.transform.Find("Inventory");
        if (inven == null)
        {
            Debug.LogWarning("Inventory Class cannot find the Slots Area.", inven);
            return;
        }

        // Get all the Pages
        int numPages = inven.childCount;
        inventoryPages = new List<InventoryPage>();
        for (int i = 0; i < numPages; i++)
        {
            // Loop through the slots area for the pages
            Transform child = inven.GetChild(i);
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


    private void Update()
    {
        // Update the text in the description box
        GameObject current = eventSystem.currentSelectedGameObject;
        if (current != null && isUIActive)
        {
            descriptionBox.text = current.GetComponent<ItemUI>().GetItemDescription();
        }
        
    }


    /// <summary>
    /// Show the inventory menu by setting it's canvas as true
    /// </summary>
    public void DisplayInventory()
    {
        UI.GetComponent<Canvas>().enabled = true;
        isUIActive = true;

    }


    /// <summary>
    /// Hide the inventory menu by setting it's canvas as false
    /// </summary>
    public void HideInventory()
    { 
        UI.GetComponent<Canvas>().enabled = false;
        isUIActive = false;

    }


    /// <summary>
    /// Add a new item to the inventory, first by finding an empty spot in the inventory where 
    /// it can fit, and then adding it to the slot using AddItemToSlot
    /// </summary>
    /// <param name="itemObject"></param>
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


        //Check which page to use
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
                        Debug.Log("Chosen index is:" + i + "," + j);
                        AddItemToSlot(inventorySlots, item, i, j);
                        return;
                    }
                }
            }
        }

        // No pages were found, create a new page

        totalItems++;
    }



    /// <summary>
    /// Add an item to a designated slot(s) by marking all those slots as occupied and
    /// creating an ItemUI for its page.
    /// </summary>
    /// <param name="inventorySlots"></param>
    /// <param name="item"></param>
    /// <param name="startRow"></param>
    /// <param name="startCol"></param>
    public void AddItemToSlot(InventorySlot[,] inventorySlots, Item item, int startRow, int startCol)
    {
        // Add the item to the slot
        InventorySlot topLeft = inventorySlots[startRow, startCol];

        // Find all slots that this item will occupy
        // and set them
        for (int i = 0; i < item.itemHeight; i++)
        {
            for (int j = 0; j < item.itemWidth; j++)
            {
                // debug
                //int temp = startRow + i;
                //int t = startCol + j;
                //Debug.Log("is set to occupied:" + temp + "," + t);

                // get a slot in the designated area
                InventorySlot slot = inventorySlots[startRow + i, startCol + j];
                slot.SetOccupancy(true);
                slot.SetItem(item);
            }
        }

        // Create an itemUI in the page
        topLeft.GetParentPage().CreateItemUI(item, startRow, startCol);


    }



    /// <summary>
    /// Check if we can place an item in a specific slot/group of slots in the inventory. If
    /// there is any item in the designated area, then an item cannot be placed there at all
    /// </summary>
    /// <param name="startRow"></param>
    /// <param name="startCol"></param>
    /// <param name="inventorySlots"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    private bool CheckInventorySlot(int startRow, int startCol, InventorySlot[,] inventorySlots, int width, int height)
    {

        // Find all the slots that will be occupied by the item
        // if it's top left index was at [row,column] (if 
        // it was placed here)
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                InventorySlot slot = inventorySlots[startRow + i, startCol + j];
                // Check if the slot is occupied
                if (slot.GetOccupancy() == true)
                {
                    // Item cannot be placed here

                    //debug
                    //int temp = startRow + i;
                    //int t = startCol + j;
                    //Debug.Log("item cannot be placed at:" + startRow + "," + startCol);

                    return false;
                }
            }
        }
        // Item can be placed here
        return true;
    }



}
