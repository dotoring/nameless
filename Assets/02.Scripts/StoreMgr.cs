using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoreMgr : MonoBehaviour
{
    public GameObject storeItemPanel = null;
    public GameObject storeCardPanel = null;
    public Button storeQuitBtn = null;

    public GameObject storeItemPref = null;
    public GameObject storeCardPref = null;

    List<int> storeCardList = new List<int>();
    List<int> storeItemList = new List<int>();

    [Header("------bag------")]
    public GameObject cardBagContent = null;
    public GameObject cardPrefab = null;

    public Button bagBtn = null;
    public Button bagCloseBtn = null;
    public GameObject bag = null;

    // Start is called before the first frame update
    void Start()
    {
        GameMgr.RefreshHP();
        GameMgr.RefreshSP();
        GameMgr.RefreshGold();
        GameMgr.RefreshItems();

        StoreSetup();

        if (storeQuitBtn != null)
        {
            storeQuitBtn.onClick.AddListener(() =>
            {
                GameMgr.stage++;
                SceneManager.LoadScene("MapScene");
            });
        }

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

        //가방에 있는 카드만 동적생성
        for (int i = 0; i < GameMgr.cardBuffer.Count; i++)
        {
            foreach (KeyValuePair<int, int> temp in GameMgr.cardInBagList)
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

    // Update is called once per frame
    void Update()
    {
        
    }

    void StoreSetup()
    {
        for (int i = 0; i < 4;)  //전체 카드중 중복 없이 4개 랜덤 뽑기
        {
            int temp = Random.Range(0, GameMgr.cardBuffer.Count);
            if (storeCardList.Contains(temp))
            {
                continue;
            }
            else
            {
                storeCardList.Add(temp);
                i++;
            }
        }

        for (int i = 0; i < storeCardList.Count; i++)   //상점 카드 생성
        {
            GameObject cardTemp = Instantiate(storeCardPref);
            cardTemp.transform.SetParent(storeCardPanel.transform, false);
            cardTemp.GetComponent<StoreCardNode>().cardNum = storeCardList[i];
        }

        for (int i = 0; i < 4;)  //전체 아이템중 중복 없이 4개 랜덤 뽑기
        {
            int temp = Random.Range(0, 5);
            if(storeItemList.Contains(temp))
            {
                continue;
            }
            else
            {
                storeItemList.Add(temp);
                i++;
            }
        }

        for (int i = 0; i < storeItemList.Count; i++)   //상점 아이템 생성
        {
            GameObject itemTemp = Instantiate(storeItemPref);
            itemTemp.transform.SetParent(storeItemPanel.transform, false);
            itemTemp.GetComponent<StoreItemNode>().itemNum = storeItemList[i];
        }
    }
}
