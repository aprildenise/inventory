using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryTextHandler : MonoBehaviour
{

    private TextMeshProUGUI descriptionText;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI pageText;


    // Start is called before the first frame update
    void Awake()
    {
        // Make sure this component is attached to the right object in the scene
        if (!gameObject.name.Equals("Info"))
        {
            Debug.LogWarning("InventoryTextHandler is not attached to the right object.");
            return;
        }

        // Get the references to each text box
        // Get the name text/namebox
        Transform child = gameObject.transform.GetChild(0);
        nameText = child.GetComponent<TextMeshProUGUI>();

        // Get the description text/ descriptionbox
        child = gameObject.transform.GetChild(1);
        descriptionText = child.GetComponent<TextMeshProUGUI>();

        // Get the page text
        child = gameObject.transform.GetChild(2);
        pageText = child.GetComponent<TextMeshProUGUI>();
    }


    public void SetItemName(string name)
    {
        nameText.text = name;
    }

    public void SetItemDescription(string description)
    {
        descriptionText.text = description;
    }

    public void SetPageNumber(int currentPage, int totalPages)
    {
        currentPage++;
        string text = currentPage + "/" + totalPages;
        pageText.text = text;
    }

    public void SetTemporaryMessage(string message)
    {

        StartCoroutine(TemporaryMessage(message));
        //Debug.Log("Setting temporary message");

        //// Get the original message
        //string originalName = nameText.text;
        //string originalDesc = descriptionText.text;

        //// Set the new message
        //SetItemDescription(message);
        //StartCoroutine(Wait());

        //// Set the original descripton
        //SetItemName(originalName);
        //SetItemDescription(originalDesc);

        //Debug.Log("putting original message back");
    }


    private IEnumerator TemporaryMessage(string message)
    {
        // Get the original message
        string originalName = nameText.text;
        string originalDesc = descriptionText.text;

        // Set the new message
        SetItemName("");
        SetItemDescription(message);
        yield return new WaitForSeconds(5);

        // Set the original descripton
        SetItemName(originalName);
        SetItemDescription(originalDesc);
    }

    //IEnumerator Wait()
    //{
    //    Debug.Log("waiting....");
    //    yield return new WaitForSeconds(10);
    //    Debug.Log("done waiting");
    //}

}
