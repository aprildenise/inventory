using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{

    public string itemName;

    public Sprite itemSprite;
    public Sprite itemSpriteII;

    [TextArea]
    public string itemDescription;
    public int itemSize;
    public int itemQuantity;
    public ItemType itemType;


}

public enum ItemType
{
    Neither = 0,
    Consumable = 1,
    Equipable = 2
}