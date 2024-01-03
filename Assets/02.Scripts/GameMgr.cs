using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    //플레이어 관련변수
    public static float maxHp = 100;
    public static float curHp;
    public static float maxSp = 100;
    public static float curSp;
    public static float tempSp;
    public static int curGold = 350;

    public static int stage = 1;

    //보유 카드리스트
    public static Dictionary<int, int> cardInBagList = new Dictionary<int, int>() { 
        { 0, 0 }, { 1, 1 }, { 2, 2 }, {3, 3 }, {4, 4 }, {5, 5} 
    };

    //게임의 모든 카드 정보 가져오기
    [SerializeField] CardSO cardSO = null;
    public static List<Card> cardBuffer = new List<Card>();

    public static Image hpBar;
    public static Image spBar;
    public static Text hpTxt;
    public static Text spTxt;
    public static Text goldTxt;

    //보유 아이템 리스트
    public static List<int> itemList = new List<int>();
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

        curHp = maxHp;
        curSp = maxSp;

        cardInBagList.Add(cardInBagList.Count, 1);
        cardInBagList.Add(cardInBagList.Count, 2);
        cardInBagList.Add(cardInBagList.Count, 6);
        cardInBagList.Add(cardInBagList.Count, 7);
        cardInBagList.Add(cardInBagList.Count, 15);
        itemList.Add(0);
        itemList.Add(1);
        itemList.Add(2);
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

    public static void Heal(int val)
    {
        curHp += val;
        if( curHp > maxHp)
        {
            curHp = maxHp;
        }

        RefreshHP();
    }

    public static void RefreshHP()
    {
        GameObject hpImg = GameObject.Find("PlayerHpBar");
        hpBar = hpImg.GetComponent<Image>();
        GameObject hpText = GameObject.Find("HpTxt");
        hpTxt = hpText.GetComponent<Text>();

        hpBar.fillAmount = curHp / maxHp;
        hpTxt.text = "HP " + curHp + "/" + maxHp;
    }

    public static void SPUp(int val)
    {
        curSp += val;
        if( curSp > maxSp)
        {
            curSp = maxSp;
        }

        RefreshSP();
    }
    public static void RefreshSP()
    {
        if(curSp >= maxSp)
        {
            curSp = maxSp;
        }
        GameObject spImg = GameObject.Find("PlayerSpBar");
        spBar = spImg.GetComponent<Image>();
        GameObject spText = GameObject.Find("SpTxt");
        spTxt = spText.GetComponent<Text>();

        spBar.fillAmount = curSp / maxSp;
        spTxt.text = "SP " + curSp + "/" + maxSp;
    }

    public static void RefreshGold()
    {
        GameObject goldText = GameObject.Find("GoldTxt");
        goldTxt = goldText.GetComponent<Text>();
        goldTxt.text = curGold.ToString();
    }

    public static void RefreshItems()
    {
        GameObject itemPanel = GameObject.Find("ItemPanel");
        for(int i = 0; i < itemPanel.transform.childCount; i++)
        {
            Destroy(itemPanel.transform.GetChild(i).gameObject);
        }
        if (itemList.Count > 0)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                GameObject item = Instantiate(itemPref);
                item.transform.SetParent(itemPanel.transform, false);
                item.GetComponent<ItemNode>().itemNum = itemList[i];
            }
        }

    }

    public static void ResetGame()
    {
        stage = 1;
        cardInBagList = new Dictionary<int, int>() { 
            { 0, 0 }, { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 4 }, { 5, 5 } 
        };
        itemList.Clear();
        maxHp = 100;
        maxSp = 100;
        curHp = maxHp;
        curSp = maxSp;
        curGold = 0;
    }
}
