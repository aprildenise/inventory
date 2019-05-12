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
        moveVelocity = moveInput.normalized * moveSpeed;
    }


    private void FixedUpdate()
    {
        // Move the player based on the movement input
        rigidBody.MovePosition(rigidBody.position + moveVelocity * Time.fixedDeltaTime);
    }
}
