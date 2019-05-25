using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryPage : MonoBehaviour
{

    // References
    private Transform itemUIs; // gameobject that has all the itemUIs
    private GameObject itemUIPrefab; // prefab for an individual itemUI
    private CanvasGroup canvasGroup;
    public InventoryTextHandler inventoryText;

    // THESE MAY NOT BE NEEDED?
    [SerializeField] public EventSystem eventSystem;

    // Variables
    public int rows;
    public int columns;
    private int pageIndex;
    private InventorySlot[,] inventorySlots; // grid of slots in this page
    private int slotsUsed; // total number of slots that are occupied in this page


    // MAY NOT BE NEEDED
    //private bool isMovingUI; // For moving UI
    //private Vector2 moveInput;
    //private ItemUI movingUI;

    // ALSO MAY NOT BE NEEDED
    private List<ItemUI> itemUIsList; // list of displayed items in this page


    // Start is called before the first frame update
    void Awake()
    {
        // Setups

        // EventSystem reference
        //if (eventSystem == null)
        //{
        //    Debug.LogWarning("Inventory Class is missing a reference to the Event System that manages it.");
        //    return;
        //}

        // Set up this inventory page
        inventorySlots = new InventorySlot[rows, columns];
        Transform slots = transform.GetChild(0);
        slotsUsed = 0;

        // Find all the inventory slots
        int i = 0;
        while (i < slots.transform.childCount)
        {
            for (int j = 0; j < rows; j++)
            {
                // Begin a new row in inventorySlots
                for (int k = 0; k < columns; k++)
                {
                    // Add the child that's in this column

                    // Debugging check
                    if (i > slots.transform.childCount - 1)
                    {
                        Debug.LogWarning("There are not enough inventory slots in the inventory.");
                        return;
                    }

                    Transform child = slots.transform.GetChild(i);
                    InventorySlot component = child.gameObject.GetComponent<InventorySlot>();
                    component.SetParentPage(gameObject.GetComponent<InventoryPage>());
                    component.SetIndex(j, k);

                    // Debugging check
                    if (component == null)
                    {
                        Debug.LogWarning("An inventory slot does not have the InventorySlot component", child);
                        return;
                    }
                    
                    inventorySlots[j, k] = component;
                    i++;
                }
            }
        }

        // Find all itemUIs (if any) (MAY NOT BE NEEDED)
        itemUIs = transform.GetChild(1);
        if (itemUIs.transform.childCount == 0)
        {
            // There are no children. Just define the itemUIs list
            itemUIsList = new List<ItemUI>();
        }


        // Load the itemUIPrefab
        itemUIPrefab = Resources.Load<GameObject>("Prefabs/ItemUI");

        // Get the canvas renderer
        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        // Done!
        Debug.Log("Successfully Init Inventory Page");

    }



    /// <summary>
    /// Hide the current page by setting its canvas renderer as disabled.
    /// </summary>
    public void HidePage()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;
        Debug.Log("HID PAGE");
    }


    /// <summary>
    /// Display the current page by setting its canvas renderer as enabled and choosing
    /// which GameObject is to be selected by the eventSystem
    /// </summary>
    public void DisplayPage()
    {

        // Choose the first item available in the page to be the eventsystem's first selected gameobject
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                InventorySlot slot = inventorySlots[i, j];
                if (slot.GetParentItemUI() != null)
                {
                    // Use this as the selected object and break out of the loops
                    eventSystem.SetSelectedGameObject(slot.GetParentItemUI().gameObject);

                    // Debug
                    //Debug.Log(eventSystem.currentSelectedGameObject);

                    i = 10000000;
                    break;
                }
            }
        }

        // Display the page
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = true;

    }



    /// <summary>
    /// Get the number of slots that are occupied/used by an item that's already on it
    /// </summary>
    /// <returns></returns>
    public int GetSlotsUsed()
    {
        return slotsUsed;
    }


    //public void SetSlotsUsed(int slots)
    //{
    //    slotsUsed = slots;
    //}



    /// <summary>
    /// Return the 2d array of inventory slots that this page has.
    /// </summary>
    /// <returns></returns>
    public InventorySlot[,] GetInventorySlots()
    {
        return inventorySlots;
    }





    /// <summary>
    /// Used when the player is moving an itemUI. When called, check if the player can place
    /// the itemUI at the spot it's in. 
    /// </summary>
    /// <param name="itemUI"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool PlaceItemUI(ItemUI itemUI, Vector2 position)
    {
        // Get the slot where the current UI is hovering over
        Vector2 index = GetSlotFromPosition(position);
        InventorySlot originalSlot = itemUI.GetTopLeftSlot();
        //Debug.Log("move to:" + index);

        // Remove the data from the itemUI's previous spots
        RemoveItemFromSlot(itemUI);
        slotsUsed -= itemUI.GetItem().itemHeight * itemUI.GetItem().itemHeight;

        // Check if we can place the ui in that slot
        Item item = itemUI.GetItem();
        if (!CheckInventorySlot((int)index.y, (int)index.x, item.itemWidth, item.itemHeight))
        {
            Debug.Log("Cannot place here");
            // Give the reference to the slot back so it can call this method again
            itemUI.SetTopLeftSlot(originalSlot);
            return false;
        }
        else
        {
            Debug.Log("Can place here");

            // Then add the itemUI to this new slot
            AddItemToSlot(itemUI.GetItem(), (int)index.y, (int)index.x, itemUI);
            //Debug.Log("topleftslot:" + itemUI.GetTopLeftSlot());
            return true;

        }

    }



    /// <summary>
    /// Given a position in the inventory UI, convert the position to an index on the grid
    /// that corresponds to that position. Define the indeces as a Vector2.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Vector2 GetSlotFromPosition(Vector2 position)
    {
        // Convert the position to indices in the inventorySlot grid
        int x = (int)(Mathf.Abs(position.x - 20) / 60f);
        int y = (int)(Mathf.Abs(position.y + 20) / 60f);

        //Debug.Log("placed at index at:" + y + "," + x);

        return new Vector2(x, y);

    }


    /// <summary>
    /// Add an item to this page
    /// </summary>
    /// <param name="item"></param>
    public bool AddItemToPage(Item item)
    {

        //Debug.Log("Adding item to page...");

        //InventorySlot[,] inventorySlots = inventoryPage.GetInventorySlots();
        //int rows = inventoryPage.rows;
        //int columns = inventoryPage.columns;
        int width = item.itemWidth;
        int height = item.itemHeight;
        int slots = width * height;
        slotsUsed += slots;

        // Find which slot we can place it in
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Check if we can place the item starting at this slot
                if (CheckInventorySlot(i, j, width, height))
                {
                    // Place the item here
                    //Debug.Log("(add item to page) Chosen index is:" + i + "," + j);

                    // Make sure to make a new itemUi for it as well
                    ItemUI itemUI = CreateItemUI(item, i,j);

                    AddItemToSlot(item, i, j, itemUI);
                    //Debug.Log("slotsleft:" + slotsUsed);
                    return true;
                }
            }
        }

        // Could not place at this page. Return false so that the system can 
        // find another page to place it in
        
        return false;
    }


    /// <summary>
    /// Remove an item from designated slots by marking all those slots as not occupied
    /// and a removing the corresponding references. Note that this does not destroy the 
    /// given UI.
    /// </summary>
    /// <param name="itemUI"></param>
    private void RemoveItemFromSlot(ItemUI itemUI)
    {
        // Get the item from the itemUI
        Item item = itemUI.GetItem();
        int width = item.itemWidth;
        int height = item.itemHeight;

        // Get the top left slot
        InventorySlot topLeft = itemUI.GetTopLeftSlot();

        // Remove the topLeftSlot from the itemUI
        itemUI.SetTopLeftSlot(null);

        // Find all the slots that this item occupies
        // and set them
        Vector2 index = topLeft.GetIndex();
        int startRow = (int)index.y;
        int startCol = (int)index.x;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {

                // Debug
                //int y = startRow + i;
                //int x = startCol + j;
                //Debug.Log(gameObject.name + " set " + y + "," + x + " to NOT occupied");

                // Get the slot at this place
                InventorySlot slot = inventorySlots[startRow + i, startCol + j];

                // Set its references to remove the item from it
                slot.SetOccupancy(false);
                slot.SetParentItemUI(null);
            }
        }

        // Done!
    }


    /// <summary>
    /// Add an item to a designated slot(s) by marking all those slots as occupied and
    /// adding the correct itemUI references to it. Note that this does not create a new itemUI
    /// </summary>
    /// <param name="item"></param>
    /// <param name="startRow"></param>
    /// <param name="startCol"></param>
    /// <param name="itemUI"></param>
    private void AddItemToSlot(Item item, int startRow, int startCol, ItemUI itemUI)
    {
        Debug.Log("Adding item to slot...");

        // Add the item to the slot
        InventorySlot topLeft = inventorySlots[startRow, startCol];


        // Create an itemUI in the page
        //ItemUI ui = CreateItemUI(item, startRow, startCol);
        itemUI.SetTopLeftSlot(topLeft);

        // Find all slots that this item will occupy
        // and set them
        for (int i = 0; i < item.itemHeight; i++)
        {
            for (int j = 0; j < item.itemWidth; j++)
            {
                // Debug
                //int y = startRow + i;
                //int x = startCol + j;
                //Debug.Log(gameObject.name + " set " + y + "," + x + " to occupied (additemtoslot)");

                // get a slot in the designated area
                InventorySlot slot = inventorySlots[startRow + i, startCol + j];
                slot.SetOccupancy(true);
                slot.SetParentItemUI(itemUI);
                //slot.SetItem(item);
            }
        }

        // Done!

    }




    /// <summary>
    /// Check if we can place an item in a specific slot/group of slots in the inventory. If
    /// there is any item in the designated area, then an item cannot be placed there at all
    /// </summary>
    /// <param name="startRow"></param>
    /// <param name="startCol"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    private bool CheckInventorySlot(int startRow, int startCol, int width, int height)
    {

        // Find all the slots that will be occupied by the item
        // if it's top left index was at [row,column] (if 
        // it was placed here)
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {

                int checkRows = startRow + i;
                int checkCols = startCol + j;
                
                // Check if this is within the boundaries of the grid
                if (checkRows < 0 || checkRows > rows - 1)
                {
                    // Item cannot be placed here
                    return false;
                }
                if (checkCols < 0 || checkCols > columns - 1)
                {
                    // Item cannot be placed here
                    return false;
                }

                // Check if the slot is occupied
                InventorySlot slot = inventorySlots[checkRows, checkCols];
                //Debug.Log("Checking:" + checkRows + "," + checkCols);

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



    /// <summary>
    /// Create a new ItemUI for the given item, and place it at the given row
    /// and column.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="startRow"></param>
    /// <param name="startCol"></param>
    private ItemUI CreateItemUI(Item item, int startRow, int startCol)
    {
        // Create the new prefab
        GameObject newUI = (GameObject)Instantiate(itemUIPrefab, itemUIs);

        // Add the sprites to the component
        ItemUI component = newUI.GetComponent<ItemUI>();

        // Assign its item
        component.SetItem(item);

        // Find its size and position
        float y = (float)20 + (startRow * 50) + (startRow * 10);
        y = y * - 1;
        float x = (float)20 + (startCol * 50) + (startCol * 10);
        float width = (float)(50 * item.itemWidth) + (10 * (item.itemWidth - 1));
        float height = (float)(50 * item.itemHeight) + (10 * (item.itemHeight - 1));

        // Assign its size and position
        component.SetPosition(x, y);
        component.SetUISize(width, height);

        //Debug.Log("(create item ui) UI will be placed at index:" + startRow + "," + startCol);

        // Add it to the item UIs
        //itemUIs.Add(component);

        // Make sure the component has its passed down references
        component.inventoryText = inventoryText;
        component.eventSystem = eventSystem;

        // Return the created UI
        return component;

    }



    public void SetPageIndex(int num)
    {
        pageIndex = num;
    }


    public int GetPageIndex()
    {
        return pageIndex;
    }

}
