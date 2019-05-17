using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPage : MonoBehaviour{


    public int rows;
    public int columns;
    private InventorySlot[,] inventorySlots;
    

    // Start is called before the first frame update
    void Start()
    {
        inventorySlots = new InventorySlot[rows, columns];
        // Find all the inventory slots
        int i = 0;
        while (i < transform.childCount)
        {
            for (int j = 0; j < rows; j++)
            {
                // Begin a new row in inventorySlots
                for (int k = 0; k < columns; k++)
                {
                    // Add child to this column
                    Transform child = transform.GetChild(i);
                    InventorySlot component = child.gameObject.GetComponent<InventorySlot>();
                    if (component == null)
                    {
                        //Debug.LogWarning("An inventory slot does not have the InventorySlot component", child);
                        return;
                    }
                    //Debug.Log("j:" + j + ".k:" + k + ".name:" + component.name);
                    inventorySlots[j, k] = component;
                    i++;
                }
            }
        }
        Debug.Log("Successfully Init Inventory Page");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public InventorySlot[,] GetInventorySlots()
    {
        return inventorySlots;
    }
}
