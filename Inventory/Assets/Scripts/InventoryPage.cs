using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryPage : MonoBehaviour
{

    // References
    private Transform itemUIParent; // gameobject that has all the itemUIs
    private GameObject itemUIPrefab; // prefab for an individual itemUI
    private CanvasGroup canvasGroup;

    // THESE MAY NOT BE NEEDED?
    [SerializeField] public EventSystem eventSystem; // May not be needed
    public TextMeshProUGUI descriptionBox;
    public TextMeshProUGUI nameBox;

    // Variables
    public int rows;
    public int columns;
    private InventorySlot[,] inventorySlots; // grid of slots in this page
    private int slotsUsed; // total number of slots that are occupied in this page
    private bool isMovingUI;

    // ALSO MAY NOT BE NEEDED
    private List<ItemUI> itemUIs; // list of displayed items in this page


    // Start is called before the first frame update
    void Awake()
    {
        // Setups

        // EventSystem reference
        if (eventSystem == null)
        {
            Debug.LogWarning("Inventory Class is missing a reference to the Event System that manages it.");
            return;
        }

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

        // Find all itemUIs (if any)
        itemUIParent = transform.GetChild(1);
        if (itemUIParent.transform.childCount == 0)
        {
            // There are no children. Just define the itemUIs list
            itemUIs = new List<ItemUI>();
        }


        // Load the itemUIPrefab
        itemUIPrefab = Resources.Load<GameObject>("Prefabs/ItemUI");

        // Get the canvas renderer
        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        // Done!
        Debug.Log("Successfully Init Inventory Page");

    }


    public void MoveUI(ItemUI ui)
    {
        Debug.Log("MOVING A UI. AAAHH!!!");
        isMovingUI = true;

    }




    /// <summary>
    /// Hide the current page by setting its canvas renderer as disabled.
    /// </summary>
    public void HidePage()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;
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
                    Debug.Log(eventSystem.currentSelectedGameObject);

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



    /// <summary>
    /// Return the 2d array of inventory slots that this page has.
    /// </summary>
    /// <returns></returns>
    public InventorySlot[,] GetInventorySlots()
    {
        return inventorySlots;
    }




    /// <summary>
    /// Add an item to this page
    /// </summary>
    /// <param name="item"></param>
    public void AddItemToPage(Item item)
    {

        Debug.Log("Adding item to page...");

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
                    Debug.Log("Chosen index is:" + i + "," + j);
                    AddItemToSlot(item, i, j);
                    return;
                }
            }
        }
    }



    /// <summary>
    /// Add an item to a designated slot(s) by marking all those slots as occupied and
    /// creating an ItemUI for its page.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="startRow"></param>
    /// <param name="startCol"></param>
    private void AddItemToSlot(Item item, int startRow, int startCol)
    {
        Debug.Log("Adding item to slot...");

        // Add the item to the slot
        InventorySlot topLeft = inventorySlots[startRow, startCol];


        // Create an itemUI in the page
        ItemUI ui = CreateItemUI(item, startRow, startCol);
        ui.SetTopLeftSlot(topLeft);

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
                slot.SetParentItemUI(ui);
                //slot.SetItem(item);
            }
        }


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
        GameObject newUI = (GameObject)Instantiate(itemUIPrefab, itemUIParent);

        // Add the sprites to the component
        ItemUI component = newUI.GetComponent<ItemUI>();

        // Assign its item
        component.SetItem(item);

        // Find its size and position
        float y = (float)20 + (startRow * 50) + (startRow * 10);
        float x = (float)20 + (startCol * 50) + (startCol * 10);
        float width = (float)(50 * item.itemWidth) + (10 * (item.itemWidth - 1));
        float height = (float)(50 * item.itemHeight) + (10 * (item.itemHeight - 1));

        // Assign its size and position
        component.SetPosition(x, y);
        component.SetUISize(width, height);

        // Add it to the item UIs
        //itemUIs.Add(component);

        // Make sure the component has its passed down references
        component.descriptionBox = descriptionBox;
        component.nameBox = nameBox;
        component.eventSystem = eventSystem;

        // Return the created UI
        return component;

    }


}
