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
}
