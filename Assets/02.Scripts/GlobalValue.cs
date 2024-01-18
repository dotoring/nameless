using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalValue
{
    public static float maxHp;
    public static float curHp;
    public static float maxSp;
    public static float curSp;
    public static int curGold;
    public static int stage;

    public static Dictionary<int, int> ownCards = new Dictionary<int, int>();
    public static List<int> ownItems = new List<int>();

    public static void NewGameData()
    {
        maxHp = 100;
        curHp = 100;
        maxSp = 100;
        curSp = 100;
        curGold = 0;
        stage = 1;
        ownCards = new Dictionary<int, int>() {
            { 0, 0 }, { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 4 }, { 5, 5 }, { 6, 6 }
        };
        ownItems.Clear();
    }

    public static void SaveGameData()
    {
        PlayerPrefs.SetFloat("MaxHP", maxHp);
        PlayerPrefs.SetFloat("CurHp", curHp);
        PlayerPrefs.SetFloat("MaxSP", maxSp);
        PlayerPrefs.SetFloat("CurSP", curSp);
        PlayerPrefs.SetInt("CurGold", curGold);
        PlayerPrefs.SetInt("Stage", stage);

        string keyBuff = "";

        //보유한 카드 목록 저장
        int count = 0;
        PlayerPrefs.SetInt("CardListCount", ownCards.Count);
        foreach(KeyValuePair<int, int> card in ownCards)
        {
            keyBuff = string.Format("Card_key_{0}", count);
            PlayerPrefs.SetInt(keyBuff, card.Key);
            keyBuff = string.Format("Card_num_{0}", count);
            PlayerPrefs.SetInt(keyBuff, card.Value);
            count++;
        }

        //보유한 아이템 목록 저장
        PlayerPrefs.SetInt("ItemListCount", ownItems.Count);
        for (int i = 0; i < ownItems.Count; i++)
        {
            keyBuff = string.Format("Item_{0}", i);
            PlayerPrefs.SetInt(keyBuff, ownItems[i]);
        }
    }

    public static void LoadGameData()
    {
        maxHp = PlayerPrefs.GetFloat("MaxHP");
        curHp = PlayerPrefs.GetFloat("CurHp");
        maxSp = PlayerPrefs.GetFloat("MaxSP");
        curSp = PlayerPrefs.GetFloat("CurSP");
        curGold = PlayerPrefs.GetInt("CurGold");
        stage = PlayerPrefs.GetInt("Stage");

        string keyBuff = "";

        //보유한 카드 목록 불러오기
        ownCards.Clear();
        int cardCount = PlayerPrefs.GetInt("CardListCount");
        for(int i = 0; i < cardCount; i++)
        {
            keyBuff = string.Format("Card_key_{0}", i);
            int cardKey = PlayerPrefs.GetInt(keyBuff);
            keyBuff = string.Format("Card_num_{0}", i);
            int cardNum = PlayerPrefs.GetInt(keyBuff);
            ownCards.Add(cardKey, cardNum);
        }

        //보유한 아이템 목록 불러오기
        ownItems.Clear();
        int itemCount = PlayerPrefs.GetInt("ItemListCount");
        for (int i = 0; i < itemCount; i++)
        {
            keyBuff = string.Format("Item_{0}", i);
            int itemKey = PlayerPrefs.GetInt(keyBuff);
            ownItems.Add(itemKey);
        }
    }
}
