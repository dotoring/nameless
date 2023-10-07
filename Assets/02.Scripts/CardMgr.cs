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
        //-----------------------������ ����� ī�� ���� �ܰ迡�� ī�� Ŭ�� ��-------------------------
        if(BattleMgr.phase == Phase.cardSelect)
        {
            if (BattleMgr.selectedCardList.Count <= 2 && isSelected == false)    //3�� �̻� ����x, ���õ��� ���� ī�带 ������ ��
            {
                if (GameMgr.curSp >= cardSP || this.tag == "UtilCard")    //ī�� ����� SP�� ���� ��
                {
                    BattleMgr.selectedCardList.Add(gameObject); //���õ� ī�� ����Ʈ�� �߰�
                    isSelected = true;
                    GameObject selectedCardArea = GameObject.Find("SelectedCardList");
                    GameObject card = Instantiate(gameObject);
                    card.transform.localScale = new Vector3(0.72f, 0.72f, 1f);
                    card.transform.SetParent(selectedCardArea.transform, false);   //ī�� ���� �� ���õ� ī����UI�� ǥ��

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
            else if (isSelected == true) //���õ� ī�带 ������ ��(���� ���)
            {
                for (int i = 0; i < BattleMgr.selectedCardList.Count; i++)
                {
                    if (BattleMgr.selectedCardList[i].GetComponent<CardMgr>().cardNum == cardNum)
                    {
                        BattleMgr.selectedCardList.RemoveAt(i); //���õ� ī�� ����Ʈ���� ���� ī�带 ã�Ƽ� ����
                        break;
                    }
                }

                GameObject obj = GameObject.Find("BattleMgr");
                BattleMgr battleMgr = obj.GetComponent<BattleMgr>();
                battleMgr.RefreshSelectedCardArea();    //���õ� ī����UI ���ΰ�ħ(���� ����� ī�� ����)

                Transform[] child = battleMgr.cardBagContent.GetComponentsInChildren<Transform>();
                for (int i = 1; i < child.Length; i++)
                {
                    CardMgr cardMgr = child[i].gameObject.GetComponent<CardMgr>();
                    if (cardMgr != null && cardMgr.cardNum == cardNum && cardMgr.isSelected == true)
                    {
                        cardMgr.isSelected = false; //������ ī��� �� ������� �� ī�带 ã�Ƽ� isSelect ����
                        break;
                    }
                }

                if (this.tag != "UtilCard")
                {
                    GameMgr.curSp += cardSP; //����ߴ� SP ��ȯ
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
        //---------------------�� ī�� ���� �ܰ迡�� ī�� Ŭ�� ��-----------------------
        else if (BattleMgr.phase == Phase.battleEndNewCard)  
        {
            GameMgr.cardInBagList.Add(cardNum); //���濡 �� ī�� �߰�
            BattleMgr.phase = Phase.battleEndResult;    //���â �ܰ�� �Ѿ��
        }
        //--------------------�ʿ��� ī�� Ŭ�� ��-----------------------------
        else if(BattleMgr.phase == Phase.map)
        {
            return; //����x
        }
    }
}
