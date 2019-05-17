using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{


    private Image imageComponent;

    private void Start()
    {
        imageComponent = this.gameObject.GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogWarning("Image Component is missing from an Inventory Slot.", gameObject);
        }
    }

}
