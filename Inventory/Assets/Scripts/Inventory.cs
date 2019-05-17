using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    // References
    [SerializeField] private GameObject UI;

    // Variables
    private List<Item> inventory;
    private int totalItems;
    

    private void Start()
    {

        inventory = new List<Item>();
        totalItems = 0;
    }

    public void AddToInventory(Item item)
    {
        inventory.Add(item);
        totalItems++;
    }

}
