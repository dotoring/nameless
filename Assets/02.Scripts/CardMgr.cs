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

        if(card.coords.Length != 0)     //��ǥ ���� �� ���� ��!
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
            if (BattleMgr.selectedCardList.Count <= 2 && isSelected == false)    //3�� �̻� ����x, ���õ��� ���� ī�带 ������ ��
            {
                if (GameMgr.curSp >= cardSP)    //ī�� ����� SP�� ���� ��
                {
                    BattleMgr.selectedCardList.Add(gameObject); //���õ� ī�� ����Ʈ�� �߰�
                    isSelected = true;
                    GameObject selectedCardArea = GameObject.Find("SelectedCardList");
                    GameObject card = Instantiate(gameObject);
                    card.transform.SetParent(selectedCardArea.transform);   //ī�� ���� �� ���õ� ī����UI�� ǥ��

                    GameMgr.curSp -= cardSP;
                    GameMgr.RefreshSP();
                }
            }
            else if (isSelected == true) //���õ� ī�带 ������ ��
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
                    if (cardMgr != null && cardMgr.cardNum == cardNum)
                    {
                        cardMgr.isSelected = false; //������ ī��� �� ������� �� ī�带 ã�Ƽ� isSelect ����
                    }
                }

                GameMgr.curSp += cardSP;    //����ߴ� SP ��ȯ
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
