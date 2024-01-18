using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum CardType
{
    moveCard = 0,
    attCard = 1,
    utilCard = 2
}

public class CardMgr : MonoBehaviour
{
    public int cardKey;
    public int cardNum;
    public Image cardArea;
    public Text cardName;
    public int cardDmg;
    public Text cardDmgTxt;
    public int cardSP;
    public Text cardSPTxt;
    public Text cardInfoTxt;
    public List<Coords> cardCoords;
    public CardType cardType;
    public int utilType;
    public bool isSelected = false;

    public Image selectedEffect;
    Image[] images;

    Button cardBtn;

    // Start is called before the first frame update
    void Start()
    {
        cardBtn = GetComponent<Button>();

        images = gameObject.GetComponentsInChildren<Image>();

        if (cardBtn != null)
        {
            cardBtn.onClick.AddListener(CardClick);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected)  //카드 선택시 선택된 이펙트 효과넣기
        {
            selectedEffect.gameObject.SetActive(true);
        }
        else
        {
            selectedEffect.gameObject.SetActive(false);
        }

        if(BattleMgr.phase == Phase.cardSelect)
        {
            if (StatusUIMgr.tempSp < cardSP && !isSelected && tag != "UtilCard")
            {
                foreach (Image image in images)
                {
                    image.color = new Color(0.5f, 0.5f, 0.5f);
                }
            }
            else
            {
                foreach (Image image in images)
                {
                    image.color = new Color(1.0f, 1.0f, 1.0f);
                }
            }
        }
    }

    public void SetCard(Card card, int key)
    {
        cardKey = key; //해당 카드의 고유 값(중복 x)
        cardType = card.cardType;
        cardNum = card.cardNum; //카드 종류 구별용 번호(중복 o)
        cardName.text = card.cardName;
        cardDmg = card.cardDmg;
        cardDmgTxt.text = card.cardDmg.ToString();
        cardSP = card.cardSP;
        cardSPTxt.text = card.cardSP.ToString();
        cardArea.sprite = card.cardImage;
        utilType = card.utileType;
        cardInfoTxt.text = card.cardInfo;

        if(card.coords.Length != 0)
        {
            for (int i = 0; i < card.coords.Length; i++)
            {
                cardCoords.Add(card.coords[i]);
            }
        }

        if(cardType == CardType.moveCard)
        {
            gameObject.tag = "MoveCard";
        }
        else if(cardType == CardType.attCard)
        {
            gameObject.tag = "AttCard";
        }
        else if(cardType == CardType.utilCard)
        {
            gameObject.tag = "UtilCard";
            cardSPTxt.text = "+" + card.cardSP.ToString();
        }
    }

    void CardClick()
    {
        //-----------------------전투에 사용할 카드 선택 단계에서 카드 클릭 시-------------------------
        if (BattleMgr.phase == Phase.cardSelect)
        {
            GameObject obj = GameObject.Find("BattleMgr");
            BattleMgr battleMgr = obj.GetComponent<BattleMgr>();

            if (BattleMgr.selectedCardOrder.Count <= 2 && isSelected == false)    //3개 이상 선택x, 선택되지 않은 카드를 눌렀을 때
            {
                if (StatusUIMgr.tempSp >= cardSP || this.tag == "UtilCard")    //카드 사용할 SP가 있을 때
                {
                    BattleMgr.selectedCardOrder.Add(gameObject); //선택된 카드 순서 리스트에 추가
                    BattleMgr.selectedCardDic.Add(cardKey, cardNum); //선택된 카드 딕셔너리에 추가
                    isSelected = true;
                    battleMgr.RefreshSelectedCardArea(); //선택된카드 화면 새로고침

                    if(this.tag != "UtilCard")
                    {
                        StatusUIMgr.tempSp -= cardSP;
                    }
                    else
                    {
                        StatusUIMgr.tempSp += cardSP;
                    }
                    StatusUIMgr.RefreshSP();
                }
            }
            else if (isSelected == true) //선택된 카드를 눌렀을 때(선택 취소)
            {
                BattleMgr.selectedCardOrder.Remove(gameObject); //선택된 카드 순서 리스트에서 제거
                BattleMgr.selectedCardDic.Remove(cardKey);  //선택된 카드 딕셔너리에서 제거
                battleMgr.RefreshSelectedCardArea(); //선택된카드 화면 새로고침

                isSelected = false;

                if (this.tag != "UtilCard")
                {
                    StatusUIMgr.tempSp += cardSP; //사용했던 SP 반환
                }
                else
                {
                    StatusUIMgr.tempSp -= cardSP;
                    if(StatusUIMgr.tempSp <= 0)
                    {
                        battleMgr.ClearSelectedCard();
                        battleMgr.RefreshSelectedCardArea(); //선택된카드 화면 새로고침
                        StatusUIMgr.SetTempSP();
                    }
                }
            }
        }
        //---------------------새 카드 선택 단계에서 카드 클릭 시-----------------------
        else if (BattleMgr.phase == Phase.battleEndNewCard)  
        {
            StatusUIMgr.AddCard(cardNum); //가방에 이 카드 추가
            BattleMgr.phase = Phase.battleEndResult;    //결과창 단계로 넘어가기
        }
        //--------------------맵에서 카드 클릭 시-----------------------------
        else if(BattleMgr.phase == Phase.map)
        {
            return; //동작x
        }
    }
}
