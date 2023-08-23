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
    public int cardNum;
    public Image cardArea;
    public Text cardName;
    public int cardDmg;
    public Text cardDmgTxt;
    public int cardSP;
    public Text cardSPTxt;
    public List<Coords> cardCoords;
    static CardType cardType;
    public bool isSelected = false;

    public Button test;

    // Start is called before the first frame update
    void Start()
    {
        if(test != null)
        {
            test.onClick.AddListener(CardClick);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCard(Card card)
    {
        cardNum = card.cardNum;
        cardName.text = card.cardName;
        cardDmg = card.cardDmg;
        cardDmgTxt.text = card.cardDmg.ToString();
        cardSP = card.cardSP;
        cardSPTxt.text = card.cardSP.ToString();
        cardType = card.cardType;
        cardArea.sprite = card.cardImage;

        if(card.coords.Length != 0)     //좌표 통일 후 지울 것!
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
        }
    }

    void CardClick()
    {
        if(BattleMgr.phase == Phase.cardSelect)
        {
            if (BattleMgr.selectedCardList.Count <= 2 && isSelected == false)    //3개 이상 선택x, 선택되지 않은 카드를 눌렀을 때
            {
                if (GameMgr.curSp >= cardSP)    //카드 사용할 SP가 있을 때
                {
                    BattleMgr.selectedCardList.Add(gameObject); //선택된 카드 리스트에 추가
                    isSelected = true;
                    GameObject selectedCardArea = GameObject.Find("SelectedCardList");
                    GameObject card = Instantiate(gameObject);
                    card.transform.SetParent(selectedCardArea.transform);   //카드 생성 후 선택된 카드목록UI에 표시

                    GameMgr.curSp -= cardSP;
                    GameMgr.RefreshSP();
                }
            }
            else if (isSelected == true) //선택된 카드를 눌렀을 때
            {
                for (int i = 0; i < BattleMgr.selectedCardList.Count; i++)
                {
                    if (BattleMgr.selectedCardList[i].GetComponent<CardMgr>().cardNum == cardNum)
                    {
                        BattleMgr.selectedCardList.RemoveAt(i); //선택된 카드 리스트에서 같은 카드를 찾아서 제거
                        break;
                    }
                }

                GameObject obj = GameObject.Find("BattleMgr");
                BattleMgr battleMgr = obj.GetComponent<BattleMgr>();
                battleMgr.RefreshSelectedCardArea();    //선택된 카드목록UI 새로고침(선택 취소한 카드 때문)

                Transform[] child = battleMgr.cardBagContent.GetComponentsInChildren<Transform>();
                for (int i = 1; i < child.Length; i++)
                {
                    CardMgr cardMgr = child[i].gameObject.GetComponent<CardMgr>();
                    if (cardMgr != null && cardMgr.cardNum == cardNum)
                    {
                        cardMgr.isSelected = false; //가방의 카드들 중 선택취소 한 카드를 찾아서 isSelect 끄기
                    }
                }

                GameMgr.curSp += cardSP;    //사용했던 SP 반환
                GameMgr.RefreshSP();
            }
        }
        else if(BattleMgr.phase == Phase.battleEndNewCard)
        {
            Debug.Log("good");
            GameMgr.cardInBagList.Add(cardNum);
            BattleMgr.phase = Phase.battleEndResult;
        }
    }
}
