using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    //게임의 모든 카드 정보 가져오기
    [SerializeField] CardSO cardSO = null;
    public static List<Card> cardBuffer = new List<Card>();

    //보유 아이템 리스트
    [SerializeField] ItemSO itemSO = null;
    public static List<Item> itemBuffer = new List<Item>();
    public static GameObject itemPref;

    public static GameMgr Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        itemPref = Resources.Load<GameObject>("Prefabs/Item");

        SetUpCardBuffer();
        SetUpItemBuffer();

    }

    void SetUpCardBuffer()
    {
        for(int i = 0; i < cardSO.cardList.Length; i++)
        {
            cardBuffer.Add(cardSO.cardList[i]);
        }
    }

    void SetUpItemBuffer()
    {
        for (int i = 0; i < itemSO.items.Length; i++)
        {
            itemBuffer.Add(itemSO.items[i]);
        }
    }
}
