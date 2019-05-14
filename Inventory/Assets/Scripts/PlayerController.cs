using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // References
    private Rigidbody2D rigidBody;


    // For movement
    [SerializeField] private float moveSpeed;
    private float dashSpeed;
    private Vector2 moveVelocity;
    [SerializeField] private float rotationSpeed;
    private Quaternion rotateDirection;

    // For interaction
    private GameObject currentInteraction;

    // Start is called before the first frame update
    void Start()
    {
        // Setups
        rigidBody = GetComponent<Rigidbody2D>();
        if (rigidBody == null)
        {
            Debug.LogWarning("PlayerController cannot find a RigidBody2D component on the object it is attached to.", rigidBody);
        }

        dashSpeed = moveSpeed * 2;
    }

    // Update is called once per frame
    void Update()
    {
        // Get movement input
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // Get rotation direction based on movement input
        if (moveInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            rotateDirection = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        moveVelocity = moveInput.normalized * moveSpeed;


        // Get interact input
        if (Input.GetButtonDown("Submit"))
        {
            // Check to see if we have collided with something to interact with
            if (currentInteraction != null)
            {
                Debug.Log("Player is about to interact with " + currentInteraction);
                currentInteraction.GetComponent<Interactable>().Interact();
            }
            else
            {
                Debug.Log("Player is not close enough to interact with anything.");
            }
        }


    }

    // Move the player based on the movement input
    private void FixedUpdate()
    {
        // Move the player based on the movement input
        rigidBody.MovePosition(rigidBody.position + moveVelocity * Time.fixedDeltaTime);
        // Rotate player character to face that direction
        transform.rotation = rotateDirection;
    }


    // Check if the player is close enough to interact with something
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


    // Check if the player has left whatever they are interacting with
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
