using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int itemNum;
    public string itemName;
    public int itemCost;
    public Sprite itemImage;
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Item[] items;
}