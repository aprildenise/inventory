using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{

    public string itemName;

    public Sprite itemSprite;
    public Sprite itemSpriteAlt;

    [TextArea]
    public string itemDescription;
    public int itemWidth;
    public int itemHeight;
    public int itemQuantity;
    public InteractType interactType;


}

public enum InteractType
{
    Neither = 0,
    Consumable = 1,
    Equipable = 2
}