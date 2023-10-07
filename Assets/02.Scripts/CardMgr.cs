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
    Card cardClass;
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

    Button cardBtn;

    // Start is called before the first frame update
    void Start()
    {
        cardBtn = GetComponent<Button>();

        if(cardBtn != null)
        {
            cardBtn.onClick.AddListener(CardClick);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCard(Card card)
    {
        cardType = card.cardType;
        cardNum = card.cardNum;
        cardName.text = card.cardName;
        cardDmg = card.cardDmg;
        cardDmgTxt.text = card.cardDmg.ToString();
        cardSP = card.cardSP;
        cardSPTxt.text = card.cardSP.ToString();
        cardArea.sprite = card.cardImage;

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
        if(BattleMgr.phase == Phase.cardSelect)
        {
            if (BattleMgr.selectedCardList.Count <= 2 && isSelected == false)    //3개 이상 선택x, 선택되지 않은 카드를 눌렀을 때
            {
                if (GameMgr.curSp >= cardSP || this.tag == "UtilCard")    //카드 사용할 SP가 있을 때
                {
                    BattleMgr.selectedCardList.Add(gameObject); //선택된 카드 리스트에 추가
                    isSelected = true;
                    GameObject selectedCardArea = GameObject.Find("SelectedCardList");
                    GameObject card = Instantiate(gameObject);
                    card.transform.localScale = new Vector3(0.72f, 0.72f, 1f);
                    card.transform.SetParent(selectedCardArea.transform, false);   //카드 생성 후 선택된 카드목록UI에 표시

                    if(this.tag != "UtilCard")
                    {
                        GameMgr.curSp -= cardSP;
                        if(GameMgr.curSp < 0)
                        {
                            GameMgr.curSp = 0;
                        }
                    }
                    else
                    {
                        GameMgr.curSp += cardSP;
                        if(GameMgr.curSp > GameMgr.maxSp)
                        {
                            GameMgr.curSp = GameMgr.maxSp;
                        }
                    }
                    GameMgr.RefreshSP();
                }
            }
            else if (isSelected == true) //선택된 카드를 눌렀을 때(선택 취소)
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
                    if (cardMgr != null && cardMgr.cardNum == cardNum && cardMgr.isSelected == true)
                    {
                        cardMgr.isSelected = false; //가방의 카드들 중 선택취소 한 카드를 찾아서 isSelect 끄기
                        break;
                    }
                }

                if (this.tag != "UtilCard")
                {
                    GameMgr.curSp += cardSP; //사용했던 SP 반환
                    if (GameMgr.curSp > GameMgr.maxSp)
                    {
                        GameMgr.curSp = GameMgr.maxSp;
                    }
                }
                else
                {
                    GameMgr.curSp -= cardSP;
                    if (GameMgr.curSp < 0)
                    {
                        GameMgr.curSp = 0;
                    }
                }
                GameMgr.RefreshSP();
            }
        }
        //---------------------새 카드 선택 단계에서 카드 클릭 시-----------------------
        else if (BattleMgr.phase == Phase.battleEndNewCard)  
        {
            GameMgr.cardInBagList.Add(cardNum); //가방에 이 카드 추가
            BattleMgr.phase = Phase.battleEndResult;    //결과창 단계로 넘어가기
        }
        //--------------------맵에서 카드 클릭 시-----------------------------
        else if(BattleMgr.phase == Phase.map)
        {
            return; //동작x
        }
    }
}
