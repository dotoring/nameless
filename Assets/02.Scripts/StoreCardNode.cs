using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreCardNode : MonoBehaviour
{
    Card cardTemp;

    public int cardNum;
    public Image cardArea;
    public Text cardName;
    public int cardDmg;
    public Text cardDmgTxt;
    public int cardSP;
    public Text cardSPTxt;
    public int cardCost;
    public Text cardCostTxt;

    public Button cardBuyBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        SetStoreCard();

        if(cardBuyBtn != null)
        {
            cardBuyBtn.onClick.AddListener(BuyCard);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetStoreCard() //상점 카드 설정
    {
        cardTemp = GameMgr.cardBuffer[cardNum];
        cardNum = cardTemp.cardNum;
        cardName.text = cardTemp.cardName;
        cardDmg = cardTemp.cardDmg;
        cardDmgTxt.text =cardDmg.ToString();
        cardSP = cardTemp.cardSP;
        cardSPTxt.text = cardSP.ToString();
        cardArea.sprite = cardTemp.cardImage;
        cardCost = cardTemp.cardCost;
        cardCostTxt.text = cardCost.ToString();
    }

    void BuyCard()  //카드 구매 함수
    {
        if (StatusUIMgr.UseGold(cardCost))    //구입 가능한 골드일 경우
        {
            //골드 차감 후 보유카드리스트에 넣기
            StatusUIMgr.AddCard(cardNum);

            //가방안에 카드 오브젝트 생성
            GameObject obj = GameObject.Find("StatusUIMgr");
            StatusUIMgr statusUIMgr = obj.GetComponent<StatusUIMgr>();
            GameObject card = Instantiate(statusUIMgr.cardPrefab);
            card.transform.SetParent(statusUIMgr.cardBagContent.transform);
            CardMgr cardInfo = card.GetComponent<CardMgr>();
            cardInfo.SetCard(cardTemp, GlobalValue.ownCards.Count);

            //구매한 카드 구매불가 표시
            cardBuyBtn.gameObject.SetActive(false);
            Image[] images = gameObject.GetComponentsInChildren<Image>();
            foreach (Image image in images)
            {
                image.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }
}
