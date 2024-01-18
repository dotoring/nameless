using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatusUIMgr : MonoBehaviour
{
    public GameObject cardBagContent = null;
    public GameObject cardPrefab = null;

    public Button bagBtn = null;
    public Button bagCloseBtn = null;
    public GameObject bag = null;

    public static float tempSp;

    public static Image hpBar;
    public static Image spBar;
    public static Text hpTxt;
    public static Text spTxt;
    public static Text goldTxt;

    //설정 창
    public Button configBtn = null;
    public GameObject configPanel = null;
    public Button goTitleBtn = null;
    public Button configCloseBtn = null;

    //플레이어 아이템 관련
    //private static List<int> itemList = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        RefreshHP();
        RefreshSP();
        RefreshGold();
        RefreshItems();

        if (bagBtn != null)
        {
            bagBtn.onClick.AddListener(() =>
            {
                bag.gameObject.SetActive(true);
            });
        }

        if (bagCloseBtn != null)
        {
            bagCloseBtn.onClick.AddListener(() =>
            {
                bag.gameObject.SetActive(false);
            });
        }

        RefreshCardBag();

        if(configBtn != null)
        {
            configBtn.onClick.AddListener(() =>
            {
                configPanel.gameObject.SetActive(true);
            });
        }

        if(goTitleBtn != null)
        {
            goTitleBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("TitleScene");
            });
        }

        if(configCloseBtn != null)
        {
            configCloseBtn.onClick.AddListener(() =>
            {
                configPanel.gameObject.SetActive(false);
            });
        }
    }

    //==============체력 관련 함수들===================
    public static void Heal(int val)
    {
        GlobalValue.curHp += val;
        if (GlobalValue.curHp > GlobalValue.maxHp)
        {
            GlobalValue.curHp = GlobalValue.maxHp;
        }

        RefreshHP();
    }

    public static void PercentHeal(float val)
    {
        if (GlobalValue.curHp < GlobalValue.maxHp * val)
        {
            GlobalValue.curHp = GlobalValue.maxHp * val;
        }

        RefreshHP();
    }

    public static void IncreaseHP(int val)
    {
        GlobalValue.maxHp += val;
        GlobalValue.curHp += val;

        RefreshHP();
    }

    public static void Damage(int val)
    {
        GlobalValue.curHp -= val;
        if(GlobalValue.curHp < 0)
        {
            GlobalValue.curHp = 0;
        }

        RefreshHP();

        if (GlobalValue.curHp <= 0)
        {
            BattleMgr.phase = Phase.gameOver;
            Debug.Log("game over");
        }
    }

    public static void RefreshHP()
    {
        GameObject hpImg = GameObject.Find("PlayerHpBar");
        hpBar = hpImg.GetComponent<Image>();
        GameObject hpText = GameObject.Find("HpTxt");
        hpTxt = hpText.GetComponent<Text>();

        hpBar.fillAmount = GlobalValue.curHp / GlobalValue.maxHp;
        hpTxt.text = "HP " + GlobalValue.curHp + "/" + GlobalValue.maxHp;
    }

    //==============마나 관련 함수들===================
    public static void SetTempSP()
    {
        tempSp = GlobalValue.curSp;
    }

    public static void SPRecovery(int val)
    {
        GlobalValue.curSp += val;
        if (GlobalValue.curSp > GlobalValue.maxSp)
        {
            GlobalValue.curSp = GlobalValue.maxSp;
        }

        RefreshSP();
    }

    public static void FillSP()
    {
        GlobalValue.curSp = GlobalValue.maxSp;

        RefreshSP();
    }

    public static void UseSP(int val)
    {
        GlobalValue.curSp -= val;
        RefreshSP();
    }

    public static void IncreaseSP(int val)
    {
        GlobalValue.maxSp += val;
        GlobalValue.curSp = GlobalValue.maxSp;
        RefreshGold();
    }

    public static void RefreshSP()
    {
        if (GlobalValue.curSp >= GlobalValue.maxSp)
        {
            GlobalValue.curSp = GlobalValue.maxSp;
        }
        GameObject spImg = GameObject.Find("PlayerSpBar");
        spBar = spImg.GetComponent<Image>();
        GameObject spText = GameObject.Find("SpTxt");
        spTxt = spText.GetComponent<Text>();

        spBar.fillAmount = GlobalValue.curSp / GlobalValue.maxSp;
        spTxt.text = "SP " + GlobalValue.curSp + "/" + GlobalValue.maxSp;
    }

    //==============골드 관련 함수들===================
    public static void EarnGold(int val)
    {
        GlobalValue.curGold += val;

        RefreshGold();
    }

    public static bool UseGold(int val)
    {
        if(GlobalValue.curGold >= val)
        {
            GlobalValue.curGold -= val;
            RefreshGold();
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void RefreshGold()
    {
        GameObject goldText = GameObject.Find("GoldTxt");
        goldTxt = goldText.GetComponent<Text>();
        goldTxt.text = GlobalValue.curGold.ToString();
    }

    //==============스테이지 관련 함수들===================
    public static void NextStage()
    {
        GlobalValue.stage++;
    }

    //==============아이템 관련 함수들===================
    public static void RefreshItems()
    {
        GameObject itemPanel = GameObject.Find("ItemPanel");
        for (int i = 0; i < itemPanel.transform.childCount; i++)
        {
            Destroy(itemPanel.transform.GetChild(i).gameObject);
        }
        if (GlobalValue.ownItems.Count > 0)
        {
            for (int i = 0; i < GlobalValue.ownItems.Count; i++)
            {
                GameObject item = Instantiate(GameMgr.itemPref);
                item.transform.SetParent(itemPanel.transform, false);
                item.GetComponent<ItemNode>().itemNum = GlobalValue.ownItems[i];
            }
        }
    }

    public static void AddItem(int val)
    {
        GlobalValue.ownItems.Add(val);
        RefreshItems();
    }

    public static void RemoveItem(int val)
    {
        GlobalValue.ownItems.Remove(val);
    }

    public static int ItemCount()
    {
        return GlobalValue.ownItems.Count;
    }

    //==============가방, 카드 관련 함수들===================
    public void RefreshCardBag()
    {
        //가방에 있는 카드만 동적생성
        for (int i = 0; i < GameMgr.cardBuffer.Count; i++)
        {
            foreach (KeyValuePair<int, int> temp in GlobalValue.ownCards)
            {
                if (GameMgr.cardBuffer[i].cardNum == temp.Value)
                {
                    GameObject card = Instantiate(cardPrefab);
                    card.transform.SetParent(cardBagContent.transform);
                    CardMgr cardInfo = card.GetComponent<CardMgr>();
                    cardInfo.SetCard(GameMgr.cardBuffer[i], temp.Key);
                }
            }
        }
    }

    public static void AddCard(int val)
    {
        GlobalValue.ownCards.Add(GlobalValue.ownCards.Count, val);
    }

    public void ClearIsSelected()
    {
        Transform[] child = cardBagContent.GetComponentsInChildren<Transform>();
        if (child != null)
        {
            for (int i = 1; i < child.Length; i++)
            {
                CardMgr cardMgr = child[i].gameObject.GetComponent<CardMgr>();
                if (cardMgr != null)
                {
                    cardMgr.isSelected = false;
                }
            }
        }
    }
}
