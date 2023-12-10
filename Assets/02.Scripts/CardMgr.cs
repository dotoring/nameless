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
    public List<Coords> cardCoords;
    public CardType cardType;
    public int utilType;
    public bool isSelected = false;

    public Image selectedEffect;
    Image[] images;
    public Card cardInfo;

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
        if (isSelected)  //ī�� ���ý� ���õ� ����Ʈ ȿ���ֱ�
        {
            selectedEffect.gameObject.SetActive(true);
        }
        else
        {
            selectedEffect.gameObject.SetActive(false);
        }

        if(BattleMgr.phase == Phase.cardSelect)
        {
            if (GameMgr.tempSp < cardSP && !isSelected && tag != "UtilCard")
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
        cardKey = key;
        cardType = card.cardType;
        cardNum = card.cardNum;
        cardName.text = card.cardName;
        cardDmg = card.cardDmg;
        cardDmgTxt.text = card.cardDmg.ToString();
        cardSP = card.cardSP;
        cardSPTxt.text = card.cardSP.ToString();
        cardArea.sprite = card.cardImage;
        utilType = card.utileType;

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

        cardInfo = card;
    }

    void CardClick()
    {
        //-----------------------������ ����� ī�� ���� �ܰ迡�� ī�� Ŭ�� ��-------------------------
        if (BattleMgr.phase == Phase.cardSelect)
        {
            GameObject obj = GameObject.Find("BattleMgr");
            BattleMgr battleMgr = obj.GetComponent<BattleMgr>();

            if (BattleMgr.selectedCardOrder.Count <= 2 && isSelected == false)    //3�� �̻� ����x, ���õ��� ���� ī�带 ������ ��
            {
                if (GameMgr.tempSp >= cardSP || this.tag == "UtilCard")    //ī�� ����� SP�� ���� ��
                {
                    BattleMgr.selectedCardOrder.Add(gameObject); //���õ� ī�� ���� ����Ʈ�� �߰�
                    BattleMgr.selectedCardDic.Add(cardKey, cardNum); //���õ� ī�� ��ųʸ��� �߰�
                    isSelected = true;
                    battleMgr.RefreshSelectedCardArea(); //���õ�ī�� ȭ�� ���ΰ�ħ

                    if(this.tag != "UtilCard")
                    {
                        GameMgr.tempSp -= cardSP;
                    }
                    else
                    {
                        GameMgr.tempSp += cardSP;
                    }
                    GameMgr.RefreshSP();
                }
            }
            else if (isSelected == true) //���õ� ī�带 ������ ��(���� ���)
            {
                BattleMgr.selectedCardOrder.Remove(gameObject); //���õ� ī�� ���� ����Ʈ���� ����
                BattleMgr.selectedCardDic.Remove(cardKey);  //���õ� ī�� ��ųʸ����� ����
                battleMgr.RefreshSelectedCardArea(); //���õ�ī�� ȭ�� ���ΰ�ħ

                isSelected = false;

                if (this.tag != "UtilCard")
                {
                    GameMgr.tempSp += cardSP; //����ߴ� SP ��ȯ
                }
                else
                {
                    GameMgr.tempSp -= cardSP;
                    if(GameMgr.tempSp <= 0)
                    {
                        battleMgr.ClearSelectedCard();
                        battleMgr.RefreshSelectedCardArea(); //���õ�ī�� ȭ�� ���ΰ�ħ
                        GameMgr.tempSp = GameMgr.curSp;
                    }
                }
            }
        }
        //---------------------�� ī�� ���� �ܰ迡�� ī�� Ŭ�� ��-----------------------
        else if (BattleMgr.phase == Phase.battleEndNewCard)  
        {
            GameMgr.cardInBagList.Add(GameMgr.cardInBagList.Count, cardNum); //���濡 �� ī�� �߰�
            BattleMgr.phase = Phase.battleEndResult;    //���â �ܰ�� �Ѿ��
        }
        //--------------------�ʿ��� ī�� Ŭ�� ��-----------------------------
        else if(BattleMgr.phase == Phase.map)
        {
            return; //����x
        }
    }
}
