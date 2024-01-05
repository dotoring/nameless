using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUIMgr : MonoBehaviour
{
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
    }

    public void RefreshCardBag()
    {
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
