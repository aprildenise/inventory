using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private string playerName;
    private int totalHP;
    private int currentHP;
    private int holdableWeight;

    private Item equippedItem;



    public int GetHoldableWeight()
    {
        return holdableWeight;
    }


    public void SetHoldableWeight(int newWeight)
    {
        holdableWeight = newWeight;
    }
    
}
