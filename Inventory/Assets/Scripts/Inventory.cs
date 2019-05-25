using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{

    // References
    [SerializeField] private GameObject UI;
    [SerializeField] InventoryTextHandler inventoryText;
    [SerializeField] private EventSystem eventSystem;
    private Transform inventoryPagesParent;
    private GameObject leftNavButton;
    private GameObject rightNavButton;

    private List<InventoryPage> inventoryPages;
    private GameObject pagePrefab;


    // Variables
    private List<Item> inventory;
    private int totalPages;
    private int totalItems; // May not be needed
    private bool isUIActive;
    private InventoryPage currentPage;



    private void Start()
    {
        // Setups
        inventory = new List<Item>();
        totalItems = 0;

        // Get the correct references
        // UI reference
        if (UI == null)
        {
            Debug.LogWarning("Inventory Class is missing a reference to the inventory UI.");
            return;
        }

        // inventory text reference
        if (inventoryText == null)
        {
            Debug.LogWarning("Inventory Class is missing a reference to the Inventory Text Handler");
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

        // Get references to the navigation buttons
        Transform c = menu.transform.GetChild(2);
        leftNavButton = c.gameObject;
        c = menu.transform.GetChild(3);
        rightNavButton = c.gameObject;
        // Make sure to turn them off
        leftNavButton.SetActive(false);
        rightNavButton.SetActive(false);


        // Get the reference to the inventory
        Transform inven = menu.transform.Find("Inventory");
        if (inven == null)
        {
            Debug.LogWarning("Inventory Class cannot find the Slots Area.", inven);
            return;
        }


        // Get all the Pages that currently exist
        int numPages = inven.childCount;
        inventoryPagesParent = inven;
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

            // Make sure the page has its passed down references
            inventoryPageComp.SetPageIndex(i);
            inventoryPageComp.eventSystem = eventSystem;
            inventoryPageComp.inventoryText = inventoryText;

            totalPages++;
        }


        // Set the first page as the current page
        currentPage = inventoryPages[0];
        inventoryText.SetPageNumber(0, totalPages);

        // See if we need to display the buttons
        if (totalPages > 1)
        {
            rightNavButton.SetActive(true);
        }

        // Get the reference to the page prefab
        pagePrefab = Resources.Load<GameObject>("Prefabs/InventoryPage");


        


        // Make sure to hide the inventory when done
        HideInventory();

        // Done!
        Debug.Log("Successfully Init Inventory");

    }



    public void NavigatePageLeft()
    {
        // Get the current page
        int current = currentPage.GetPageIndex();

        // Hide that page
        currentPage.HidePage();
        Debug.Log(currentPage.name);

        // Display the next page
        InventoryPage nextPage = inventoryPages[current - 1];
        nextPage.DisplayPage();
        currentPage = nextPage;

        // Check if need to show this button again (if there's another page to the right)
        if (nextPage.GetPageIndex() == 0)
        {
            // Hide the button
            leftNavButton.SetActive(false);
        }

        // Show the right
        rightNavButton.SetActive(true);

        // Show the page number
        inventoryText.SetPageNumber(current - 1, totalPages);
    }


    public void NavigatePageRight()
    {
        // Get the current page
        int current = currentPage.GetPageIndex();

        // Hide that page
        currentPage.HidePage();
        Debug.Log(currentPage.name);

        // Display the next page
        InventoryPage nextPage = inventoryPages[current + 1];
        nextPage.DisplayPage();
        currentPage = nextPage;

        // Check if need to show this button again (if there's another page to the right)
        if (totalPages - 1 == nextPage.GetPageIndex())
        {
            // Hide the button
            rightNavButton.SetActive(false);
        }

        // Show the left button 
        leftNavButton.SetActive(true);

        // Set the text
        inventoryText.SetPageNumber(current + 1, totalPages);


    }


    /// <summary>
    /// Show the inventory menu by setting it's canvas as true
    /// </summary>
    public void DisplayInventory()
    {
        UI.GetComponent<Canvas>().enabled = true;
        currentPage.DisplayPage();
        isUIActive = true;

    }


    /// <summary>
    /// Hide the inventory menu by setting it's canvas as false
    /// </summary>
    public void HideInventory()
    { 
        UI.GetComponent<Canvas>().enabled = false;
        currentPage.HidePage();
        isUIActive = false;

    }



    private InventoryPage CreatePage()
    {
        // Instantiate a new Page object
        GameObject newPage = (GameObject)Instantiate(pagePrefab, inventoryPagesParent);

        // Make sure its the parent of the Inventory
        //newPage.transform.SetParent(inventoryPagesParent, false);

        // Define all its needed references
        InventoryPage inventoryPage = newPage.GetComponent<InventoryPage>();
        //totalPages++;
        int pageNum = totalPages;
        inventoryPage.SetPageIndex(pageNum);
        inventoryPage.eventSystem = eventSystem;
        inventoryPage.inventoryText = inventoryText;

        // add the the list
        inventoryPages.Insert(pageNum, inventoryPage);

        // Hide the page from the player
        inventoryPage.HidePage();

        // Show the navigation buttons depending on the current page number
        if (currentPage.GetPageIndex() < inventoryPage.GetPageIndex())
        {
            // Show the right button
            rightNavButton.SetActive(true);
        }
        if (inventoryPage.GetPageIndex() < currentPage.GetPageIndex())
        {
            // Show the left button
            leftNavButton.SetActive(true);
        }

        totalPages++;
        return inventoryPage;

    }


    /// <summary>
    /// Add a new item to the inventory, first by finding an empty spot in the inventory where 
    /// it can fit, and then adding it to the slot using AddItemToSlot
    /// </summary>
    /// <param name="itemObject"></param>
    public void AddItemToInventory(GameObject itemObject)
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
        int slotsToUse = item.itemWidth * item.itemHeight;

        // Find where we can first place the item based on its dimensions
        // Check which page to use
        foreach (InventoryPage inventoryPage in inventoryPages)
        {
            // Find which page we can first place the item by checking which page has the correct number of
            // slots left
            int slotsLeft = (inventoryPage.rows * inventoryPage.columns) - inventoryPage.GetSlotsUsed();
            //Debug.Log("slotsleft:" + slotsLeft);
            if (slotsLeft < slotsToUse)
            {
                // Can't use this page, look at the next one
                continue;
            }
            else
            {
                // Attempt to place at this page
                bool hasAdded = inventoryPage.AddItemToPage(item);
                if (hasAdded)
                {
                    return;
                }
                
            }
        }

        // No pages were found, create a new page and place the item there
        InventoryPage newPage = CreatePage();
        newPage.AddItemToPage(item);

        // Done!
    }





}
