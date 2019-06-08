using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // References
    private Rigidbody2D rigidBody;
    [SerializeField] private Inventory inventoryManager;


    // For movement
    [SerializeField] private float moveSpeed;
    private float dashSpeed;
    private Vector2 moveVelocity;
    private Quaternion rotateDirection; // may not be needed in the future when we actually animate the player character?

    // For interaction
    private GameObject currentInteraction;
    private GameObject heldItem;
    private bool isHolding;
    private bool isInDialogue;

    // For inventory/menu management
    private bool onMenu;

    // Start is called before the first frame update
    void Start()
    {
        // Setups
        rigidBody = GetComponent<Rigidbody2D>();
        if (rigidBody == null)
        {
            Debug.LogWarning("PlayerController cannot find a RigidBody2D component on the object it is attached to.", rigidBody);
            return;
        }
        if (inventoryManager == null)
        {
            Debug.LogWarning("PlayerController cannot find the Inventory class.", inventoryManager);
            return;
        }

        isHolding = false;
        dashSpeed = moveSpeed * 2;
        onMenu = false;

        Debug.Log("Successfully Init Player Controller");
    }

    // Update is called once per frame
    void Update()
    {

        // If in dialogue, can't open up ANY menus
        if (isInDialogue)
        {
            //Debug.Log("NOPE!");
            return;
        }

        // Get inventory menu input
        if (Input.GetButtonDown("QButton"))
        {
            // Open/close inventory screen
            // Player is already in the inventory screen. Hide the inventory
            if (onMenu)
            {
                bool canHide = inventoryManager.HideInventory();
                if (canHide)
                {
                    onMenu = false;
                    return;
                }
                else
                {
                    // Player has not finished interacting with the inventory yet.
                    onMenu = true;
                    return;
                }
                
            }
            // Player is not on the inventory screen. Display the inventory
            else
            {
                onMenu = true;
                inventoryManager.DisplayInventory();
                return;
            }
        }


        // Prevent movement inputs if player is on the menu
        if (onMenu)
        {
            return;
        }


        // Get movement input
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // Get rotation direction based on movement input
        if (moveInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            rotateDirection = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        moveVelocity = moveInput.normalized * moveSpeed;


        // Get (A) interact input
        if (Input.GetButtonDown("Submit"))
        {
            // Check to see if we have collided with something to interact with
            // Player is close enough to an interactable
            if (currentInteraction != null)
            {
                // Choose how the player can interact with the interactable based on what's attached to it
                // Interactable is an item
                if (currentInteraction.GetComponent<PickupItem>() != null)
                {
                    // Player is not already holding an item
                    if (!isHolding)
                    {
                        HoldItem();
                        return;
                    }
                    
                }
                // Iteractable is an NPC
                if (currentInteraction.GetComponent<NPCController>() != null)
                {
                    Debug.Log("interacting with an npc");
                    TriggerDialogue();
                    // Enter dialogue with the NPC
                }
            }

            // Player is holding an item
            if (isHolding)
            {
                ThrowItem();
                return;
            }
        }

        //Get (X) bag input
        if (Input.GetButtonDown("EButton"))
        {
            // Player is holding an item
            if (isHolding)
            {
                // Bag the item
                BagItem(heldItem);
                heldItem = null;
                isHolding = false;

            }
        }


    }



    /// <summary>
    /// Have the player place the item in their inventory
    /// </summary>
    /// <param name="item"></param>
    private void BagItem(GameObject item)
    {
        inventoryManager.AddItemToInventory(item);

    }


    private void TriggerDialogue()
    {
        // Have the NPC enter dialogue
        NPCController npcController = currentInteraction.GetComponent<NPCController>();
        npcController.EnterDialogue();
        isInDialogue = true;
    }


    /// <summary>
    /// Have the player hold an item
    /// </summary>
    private void HoldItem()
    {
        PickupItem item = currentInteraction.GetComponent<PickupItem>();
        heldItem = item.PickUp();
        isHolding = true;

        Debug.Log("Held an item. helditem:" + heldItem);
    }


    /// <summary>
    /// Have the player thrown an item that they are holding
    /// </summary>
    private void ThrowItem()
    {
        heldItem = null;
        isHolding = false;
        Debug.Log("Threw an item. helditem:" + heldItem);
    }


    public void SetIsInDialogue(bool set)
    {
        isInDialogue = set;
    }


    // Move the player based on the movement input
    private void FixedUpdate()
    {
        // Move the player based on the movement input
        rigidBody.MovePosition(rigidBody.position + moveVelocity * Time.fixedDeltaTime);
        // Rotate player character to face that direction
        transform.rotation = rotateDirection;
    }


    /// <summary>
    /// Use event trigger in order to check what the player has collided with
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player collided with " + collision.name);
        // Check if the thing the player collided with is an interactable
        if (collision.CompareTag("Interactable"))
        {
            currentInteraction = collision.gameObject;
            Debug.Log("This collision is an interactable!");
        }
        
    }


    /// <summary>
    /// Use even trigger in order to check what the player has left the collider with
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the current thing that the player collided with is an interactable
        if (currentInteraction != null)
        {
            if (currentInteraction.CompareTag("Interactable"))
            {
                currentInteraction = null;
                Debug.Log("Player is out of range of the interactable");
            }
        }
        
    }

}
