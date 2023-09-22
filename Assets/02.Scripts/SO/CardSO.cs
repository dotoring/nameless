using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Card
{
    public int cardNum;
    public string cardName;
    public int cardDmg;
    public int cardSP;
    public Coords[] coords;
    public CardType cardType;
    public Sprite cardImage;
    public int cardCost;
    
}

[System.Serializable]
public class Coords
{
    public int x;
    public int y;
}

[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Object/CardSO")]
public class CardSO : ScriptableObject
{
    public Card[] cardList;
}
