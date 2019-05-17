using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{

    [SerializeField] private Item item;


    public GameObject PickUp()
    {
        return this.gameObject;
    }
}
