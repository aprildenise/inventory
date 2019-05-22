using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryPage : MonoBehaviour
{

    // References
    private Transform itemUIParent; // gameobject that has all the itemUIs
    private GameObject itemUIPrefab; // prefab for an individual itemUI
    [SerializeField] private EventSystem eventSystem;

    // Variables
    public int rows;
    public int columns;
    private InventorySlot[,] inventorySlots;
    private List<ItemUI> itemUIs;


    // Start is called before the first frame update
    void Start()
    {

        // EventSystem reference
        if (eventSystem == null)
        {
            Debug.LogWarning("Inventory Class is missing a reference to the Event System that manages it.");
            return;
        }

        // Set up this inventory page
        inventorySlots = new InventorySlot[rows, columns];
        Transform slots = transform.GetChild(0);

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

        Debug.Log("Successfully Init Inventory Page");

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
    /// Create a new ItemUI for the given item, and place it at the given row
    /// and column.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="startRow"></param>
    /// <param name="startCol"></param>
    public void CreateItemUI(Item item, int startRow, int startCol)
    {
        // Create the new prefab
        GameObject newUI = (GameObject)Instantiate(itemUIPrefab, itemUIParent);

        // Add the sprites to the component
        ItemUI component = newUI.GetComponent<ItemUI>();

        // Assign its item
        component.SetItem(item);

        // Find its size and position
        float y = (float) 20 + (startRow * 50) + (startRow * 10);
        float x = (float) 20 + (startCol * 50) + (startCol * 10);
        float width = (float) (50 * item.itemWidth) + (10 * (item.itemWidth - 1));
        float height = (float) (50 * item.itemHeight) + (10 * (item.itemHeight - 1));

        // Assign its size and position
        component.SetPosition(x, y);
        component.SetUISize(width, height);


    }
}
