using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{


    // References
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameText;
    private Animator animator;
    [SerializeField] private PlayerController player; // temp for now

    // Variables
    private int entryCount;
    private bool isTyping;
    private bool finishedTyping;
    private bool cancelTyping;
    private bool noMoreEntries;
    private bool dialogueComplete;
    private bool dialogueInProgress;

    private NPCController currentNPC; // temporary


    // Start is called before the first frame update
    void Start()
    {
        // Setups
        isTyping = false;
        finishedTyping = true;
        cancelTyping = false;
        noMoreEntries = true;
        dialogueComplete = true;
        dialogueInProgress = false;
    }

    // Update is called once per frame
    private void Update()
    {

        if (!dialogueInProgress)
        {
            return;
        }

        //if the user uses the submit key
        if (Input.GetButtonDown("Submit") && !noMoreEntries)
        {
            if (!isTyping)
            {
                //if the current entry has finished typing on screen, allow the player to see the next entry
                cancelTyping = false;
                DisplayDialogue(currentNPC);
            }
            else if (isTyping && !cancelTyping)
            {
                cancelTyping = true;
                //if the current entry has not finished typing, then immediately print out the entire entry without using the typewriter
            }
        }
        else if (Input.GetButtonDown("Submit") && noMoreEntries)
        {
            if (!isTyping)
            {
                //the dialogue sequence is finished. allow the user to end the dialogue
                EndDialogue();
            }
        }
    }


    public void TriggerDialogue(NPCController npc)
    {
        // Turn on the dialogue UI
        UI.GetComponent<Canvas>().enabled = true;
        dialogueComplete = false;
        dialogueInProgress = true;
        noMoreEntries = false;
        currentNPC = npc;

        // Move the dialogue box so it is over the given NPC in the scene
        // by using its collider
        BoxCollider2D collider = npc.gameObject.GetComponent<BoxCollider2D>();
        Vector3 centerPoint = new Vector3(collider.bounds.center.x, collider.bounds.center.y, 0f);
        Vector2 extend = collider.bounds.extents;
        Vector2 newPos = new Vector2(centerPoint.x - (extend.x * 2), centerPoint.y + (extend.y * 2));

        dialogueBox.transform.position = newPos;

        // Display the diaogue text
        Debug.Log("displaying dialogue in:" + gameObject.name);
        DisplayDialogue(npc);
    }


    private void DisplayDialogue(NPCController npc)
    {
        // Check if there is more dialogue to display
        string[] entries = npc.GetCharacterDialogue();
        if (entryCount > entries.Length - 1)
        {
            // No more entries
            noMoreEntries = true;
            EndDialogue();
            return;
        }

        // If there are more entries, then display the dialogue
        string entry = entries[entryCount];
        string name = npc.GetCharacterName();

        nameText.text = name;
        dialogueText.text = entry;

        // Type out the entry in the dialogue box
        StartCoroutine(TypeSentence(entry));

    }


    private IEnumerator TypeSentence(string entry)
    {
        dialogueText.text = "";
        isTyping = true;

        // Type out the sentence by looping through the entry
        foreach (char letter in entry.ToCharArray())
        {
            if (cancelTyping && isTyping)
            {
                //Check if the user would like to cancel typing.
                //If so, then break out of the loop and display the entire entry
                break;
            }
            //type out the letters
            dialogueText.text += letter;
            yield return new WaitForSeconds(.1f);

        }
        dialogueText.text = entry;
        isTyping = false;
        cancelTyping = false;
        //duim.SetCurser(true);
        entryCount++;
    }


    private void EndDialogue()
    {
        dialogueComplete = true;
        isTyping = false;
        cancelTyping = false;
        player.SetIsInDialogue(false);
        UI.GetComponent<Canvas>().enabled = false;
        Debug.Log("currentnpc:" + currentNPC);
        currentNPC.EndDialogue();
        //duim.TurnOffUI();
    }


}
