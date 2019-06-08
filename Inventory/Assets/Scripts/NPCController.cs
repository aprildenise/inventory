using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{


    // References
    // For animation
    [SerializeField] private Dialogue dialogueManager;
    private Animator animator;
    private Rigidbody2D rigidBody;
    [SerializeField] private Character character;

    // Variables
    // For movement
    private bool isFollowingPath;
    private bool isRetracingPath;
    private bool isMoving;
    private bool isHalted;
    private bool isSpeaking;

    private Vector2 moveVelocity;
    private Vector2 moveDirection;

    [SerializeField] private bool isStationary; // if the NPC remains stationary
    [SerializeField] private bool givenDirections; // if the NPC is given a specific path to follow. Else, go in random directions
    [SerializeField] private Transform[] directions; // list of directions/corners of a region that the NPC must follow/stay in
    public float defaultWaitTime;
    [SerializeField] private float[] waitTimes; // optional wait times that the NPC will follow at each direction

    private int currentGoal; // index of the NPC's current goal it must follow in its path

    public float moveSpeed;




    // Start is called before the first frame update
    void Start()
    {
        // Get animator and rigidbody
        animator = gameObject.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("NPCController is missing a reference to the Animator.");
            return;
        }
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        if (rigidBody == null)
        {
            Debug.LogWarning("NPCController is missing a refernce to the Rigidbody2D.");
        }


        // Setups
        isSpeaking = false;
        isMoving = false;
        currentGoal = 0;


        if (isStationary)
        {
            // Do not need to initialize the variables below
            return;
        }

        // Get the directions the NPC must take
        // !!ASSUME THAT NPCS WILL NOT MOVE IN RANDOM DIRECTIONS FOR NOW!!
        //Transform child = gameObject.transform.Find("Directions");
        //directions = new Transform[child.childCount];
        //for (int i = 0; i < child.childCount; i++)
        //{
        //    // Add the directions to the directions list
        //    Transform direction = child.GetChild(i);
        //    directions[i] = direction;
        //}


        // Get the waittimes
        // If there is no given wait time then give the npc the default wait times
        if (waitTimes == null)
        {
            waitTimes = new float[directions.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                waitTimes[i] = defaultWaitTime;
            }
        }


        // Choose what the NPC should do
        // !!ASSUME FOLLOWDIRECTIONS FOR NOW!!
        GetNextGoal();
        //GetNextGoal();

        // Done!

    }



    /// <summary>
    /// If the NPC has a set directions (path) to follow, get the next goal it has to move to
    /// using the currentGoal variable and setting the moveDirection and moveVelocity values.
    /// Actual movement is done in the FixedUpdate method.
    /// </summary>
    private void GetNextGoal()
    {

        // Calculate which direction to go to 
        Vector2 currentPos = gameObject.transform.position;
        Vector2 targetPos = directions[currentGoal].position;

        Vector2 heading = targetPos - currentPos;
        float distance = heading.magnitude;
        Vector2 direction = heading / distance; // Normalized direction

        // Round the direction for the animator
        Vector2 roundDir = direction;
        roundDir.x = Mathf.Round(roundDir.x);
        roundDir.y = Mathf.Round(roundDir.y);
        moveDirection = roundDir;

        // Move to direction in the FixedUpdated method
        moveVelocity = direction.normalized * moveSpeed;
        isMoving = true;
    }




    private void FixedUpdate()
    {
        // Check if the NPC is halted. If so, don't do anything to change its position
        if (isHalted)
        {
            return;
        }

        // Move the NPC to the current position in moveVelocity and update the animator
        animator.SetBool("isMoving", true);
        animator.SetFloat("moveX", moveDirection.x);
        animator.SetFloat("moveY", moveDirection.y);
        rigidBody.MovePosition(rigidBody.position + moveVelocity * Time.fixedDeltaTime);
    }



    private void Update()
    {
        // Check if the NPC is has arrived at its current goal on its path
        if (!isStationary && givenDirections)
        {
            //Debug.Log("distance:" + Vector2.Distance(transform.position, directions[currentGoal].position));

            if (Vector2.Distance(transform.position, directions[currentGoal].position) <= .1 && isMoving)
            {
                // Change the goal of the NPC
                //Debug.Log("STOP");
                isMoving = false;
                moveVelocity = Vector2.zero;
                
                // If the NPC is at the end of the path, start retracing it
                if (currentGoal == directions.Length - 1)
                {
                    isFollowingPath = false;
                    isRetracingPath = true;
                }
                // If the NPC is at the beginning of the path, start following it
                if (currentGoal == 0)
                {
                    isFollowingPath = true;
                    isRetracingPath = false;
                }

                // Choose the next goal
                if (isFollowingPath)
                {
                    currentGoal++;
                }
                if (isRetracingPath)
                {
                    currentGoal--;
                }

                //Debug.Log("currentGoal:" + currentGoal);

                StartCoroutine(Wait());
                GetNextGoal();

                return;


            }
        }
    }


    private IEnumerator Wait()
    {
        
        isHalted = true;

        // Stop moving and set the animator's idle animation
        isMoving = false;
        animator.SetBool("isMoving", false);
        animator.SetFloat("lastMoveX", moveDirection.x);
        animator.SetFloat("lastMoveY", moveDirection.y);

        // Wait a some seconds and then continue going
        yield return new WaitForSeconds(defaultWaitTime);

        isHalted = false;
    }



    public void EnterDialogue()
    {
        isHalted = true;
        isMoving = false;
        isSpeaking = true;

        // Raycast in all 4 directions to find the player and face them

        // Raycast up
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up);

        // Check if it hits the player
        if (hit.collider != null)
        {

            //Debug.Log("HIT!");
            //Debug.Log("hit:" + hit.collider.gameObject.name);

            if (hit.collider.gameObject.CompareTag("Player"))
            {
                // NPC faces up
                //Debug.Log("DOUBLE HIT!");
                animator.SetBool("isSpeaking", true);
                animator.SetFloat("lastMoveX", Vector2.up.x);
                animator.SetFloat("lastMoveY", Vector2.up.y);


            }
        }

        // Raycast right
        hit = Physics2D.Raycast(transform.position, Vector2.right);

        // Check if it hits the player
        if (hit.collider != null)
        {

            //Debug.Log("HIT!");
            //Debug.Log("hit:" + hit.collider.gameObject.name);

            if (hit.collider.gameObject.CompareTag("Player"))
            {
                // NPC faces up
                //Debug.Log("DOUBLE HIT!");
                animator.SetBool("isSpeaking", true);
                animator.SetFloat("lastMoveX", Vector2.right.x);
                animator.SetFloat("lastMoveY", Vector2.right.y);

            }
        }

        // Raycast down
        hit = Physics2D.Raycast(transform.position, Vector2.down);

        // Check if it hits the player
        if (hit.collider != null)
        {

            //Debug.Log("HIT!");
            //Debug.Log("hit:" + hit.collider.gameObject.name);

            if (hit.collider.gameObject.CompareTag("Player"))
            {
                // NPC faces up
                //Debug.Log("DOUBLE HIT!");
                animator.SetBool("isSpeaking", true);
                animator.SetFloat("lastMoveX", Vector2.down.x);
                animator.SetFloat("lastMoveY", Vector2.down.y);

            }
        }

        // Raycast left
        hit = Physics2D.Raycast(transform.position, Vector2.left);

        // Check if it hits the player
        if (hit.collider != null)
        {

            //Debug.Log("HIT!");
            //Debug.Log("hit:" + hit.collider.gameObject.name);

            if (hit.collider.gameObject.CompareTag("Player"))
            {
                // NPC faces up
                //Debug.Log("DOUBLE HIT!");
                animator.SetBool("isSpeaking", true);
                animator.SetFloat("lastMoveX", Vector2.left.x);
                animator.SetFloat("lastMoveY", Vector2.left.y);

            }
        }

        // Display the dialogue UI
        DisplayDialogue();
    }


    private void DisplayDialogue()
    {
        // Call the dialogue manager
        dialogueManager.DisplayDialogue(this);
    }



    public string[] GetCharacterDialogue()
    {
        return character.characterDialogue;
    }


    public string GetCharacterName()
    {
        return character.characterName;
    }



}
