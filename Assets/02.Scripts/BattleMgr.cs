using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Phase
{
    cardSelect,
    Action,
    battleEndNewCard,
    battleEndResult
}

public class BattleMgr : MonoBehaviour
{
    public Button bagBtn = null;
    public GameObject bag = null;
    public Button bagCloseBtn = null;

    public GameObject cardBagContent = null;
    public GameObject cardPrefab = null;

    public GameObject selectedCardArea = null;
    public static List<GameObject> selectedCardList = new List<GameObject>();

    public Button continueBtn = null;

    public GameObject monsterPrefab = null;
    public GameObject playerPrefab = null;
    PlayerCtrl playerCtrl = null;

    [Header("------Result------")]
    public GameObject resultpanel = null;
    public GameObject newCardPage = null;
    public GameObject newCardList = null;
    public GameObject resultPage = null;
    int[] newCardNums = new int[3];

    public Button nextBtn = null;

    [Header("------forQA------")]
    public Button quitBtn = null;

    public static Phase phase;
    public List<GameObject> monsters = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        GameMgr.RefreshHP();
        GameMgr.RefreshSP();

        GameObject playerObj = Instantiate(playerPrefab);
        playerCtrl = playerObj.GetComponent<PlayerCtrl>();

        for(int i = 0; i < 1; i++)  //���͵� ����
        {
            GameObject mon = Instantiate(monsterPrefab);
            MonsterCtrl monCtrl = mon.GetComponent<MonsterCtrl>();
            monCtrl.monPosX = 5;
            monCtrl.monPosY = i*2+1;
            monsters.Add(mon);
        }

        phase = Phase.cardSelect;


        //���濡 �ִ� ī�常 ��������
        int find = 0;
        for (int i = 0; i < GameMgr.cardBuffer.Count; i++)
        {
            if (GameMgr.cardBuffer[i].cardNum == GameMgr.cardInBagList[find])
            {
                GameObject card = Instantiate(cardPrefab);
                card.transform.SetParent(cardBagContent.transform);
                CardMgr cardInfo = card.GetComponent<CardMgr>();
                cardInfo.SetCard(GameMgr.cardBuffer[i]);
                find++;
                if (find >= GameMgr.cardInBagList.Count)
                {
                    break;
                }
            }  
            else
            {
                continue;
            }
        }

        if (bagBtn != null)
        {
            bagBtn.onClick.AddListener(() =>
            {
                bag.gameObject.SetActive(true);
            });
        }

        if(bagCloseBtn != null)
        {
            bagCloseBtn.onClick.AddListener(() =>
            {
                bag.gameObject.SetActive(false);
            });
        }

        if (continueBtn != null)
        {
            continueBtn.onClick.AddListener(continueBtnClick);
        }

        if(nextBtn != null)
        {
            nextBtn.onClick.AddListener(() =>
            {
                GameMgr.curSp = GameMgr.maxSp;
                SceneManager.LoadScene("MapScene");
            });
        }

        if(quitBtn != null)
        {
            quitBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MapScene");
            });
        }

        DrawRandomNewCard();
        PopNewCards();
    }

    // Update is called once per frame
    void Update()
    {
        if(phase == Phase.Action)
        {
            bag.gameObject.SetActive(false);
            bagBtn.gameObject.SetActive(false);
            continueBtn.gameObject.SetActive(false);
        }
        if(phase == Phase.cardSelect)
        {
            bagBtn.gameObject.SetActive(true);
            continueBtn.gameObject.SetActive(true);
        }
        if(phase == Phase.battleEndNewCard)
        {
            resultpanel.gameObject.SetActive(true);
        }
        if(phase == Phase.battleEndResult)
        {
            newCardPage.gameObject.SetActive(false);
            resultPage.gameObject.SetActive(true);
        }
    }

    IEnumerator ActionPhase()
    {
        for (int i = 0; i < selectedCardList.Count; i++)
        {
            CardMgr cardMgr = selectedCardList[i].GetComponent<CardMgr>();
            //-----------------�÷��̾� �ൿ---------------------
            if (selectedCardList[i].tag == "MoveCard")  //�̵�ī���� ���
            {
                playerCtrl.MoveAreaOnOff(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y, true);   //�̵� ��ġ ǥ��
                yield return new WaitForSeconds(1.0f);  //�̵�ǥ�� �� 1�� ����
                playerCtrl.MoveAreaOnOff(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y, false);  //�̵� ��ġ ǥ�� ����
                playerCtrl.Move(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y);  //��ġ�� �̵�
                for (int k = 0; k < monsters.Count; k++)
                {
                    MonsterCtrl monCtrl = monsters[k].GetComponent<MonsterCtrl>();
                    monCtrl.Move(0, 0); //��ġ �������� ����
                }
                Debug.Log("Player move");
            }
            else if (selectedCardList[i].tag == "AttCard")  //����ī���� ���
            {
                for(int j = 0; j < cardMgr.cardCoords.Count; j++)   //��� ���� ��ġ 1�ʰ� ǥ��
                {
                    playerCtrl.AttackAreaOnOff(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, true);
                }

                yield return new WaitForSeconds(1.0f);  //����ǥ�� �� 1�� ����

                for (int j = 0; j < cardMgr.cardCoords.Count; j++)  //��� ���� ��ġ ǥ�� ���� �����ϱ�
                {
                    playerCtrl.Attack(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, cardMgr.cardDmg);
                    playerCtrl.AttackAreaOnOff(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, false);
                }
                Debug.Log("attack");
            }
            else if (selectedCardList[i].tag == "UtilCard") //��ƿī���� ���
            {
                Debug.Log("heal");
            }

            yield return new WaitForSeconds(1.0f);  //�÷��̾� �ൿ �� 1�� ����

            //--------------------���� �ൿ-----------------------
            for (int k = 0; k < monsters.Count; k++)
            {
                MonsterCtrl monCtrl = monsters[k].GetComponent<MonsterCtrl>();
                monCtrl.MonsterActionArea(i);
                yield return new WaitForSeconds(1.0f);  //���� �ൿ ǥ�� �� 1�� ����
                monCtrl.MonsterAction(i);
            }
            playerCtrl.Move(0, 0);  //��ġ �������� ����

            if (i != selectedCardList.Count - 1) //������ �� ����
            {
                yield return new WaitForSeconds(1.0f);  //���� �ൿ �� 1�� ����
            }
        }
        selectedCardList.Clear();
        ClearSelectedCard();
        if(monsters.Count <= 0)
        {
            phase = Phase.battleEndNewCard;
        }
        else
        {
            phase = Phase.cardSelect;
        }
    }

    void continueBtnClick() //��� ��ư Ŭ�� ��
    {
        phase = Phase.Action;
        //���õ� ī�������Ʈ�� �����
        Transform[] childList = selectedCardArea.GetComponentsInChildren<Transform>();
        for (int i = 1; i < childList.Length; i++)
        {
            Destroy(childList[i].gameObject);
        }

        StartCoroutine(ActionPhase());
    }

    public void RefreshSelectedCardArea()   //���õ� ī�� ����UI �ʱ�ȭ
    {
        Transform[] child = selectedCardArea.GetComponentsInChildren<Transform>();
        for(int i = 1;  i < child.Length; i++)
        {
            Destroy(child[i].gameObject);
        }

        for(int i = 0; i < selectedCardList.Count; i++)
        {
            GameObject card = Instantiate(selectedCardList[i]);
            card.transform.SetParent(selectedCardArea.transform);
        }
    }

    void ClearSelectedCard()    //���õ� ī��� �ʱ�ȭ
    {
        Transform[] child = cardBagContent.GetComponentsInChildren<Transform>();
        if(child != null)
        {
            for (int i = 1; i < child.Length; i++)
            {
                CardMgr cardMgr = child[i].gameObject.GetComponent<CardMgr>();
                if(cardMgr != null)
                {
                    cardMgr.isSelected = false;
                }
            }
        }
    }

    void DrawRandomNewCard()
    {
        for(int i = 0; i < 3;)
        {
            bool isOverlap = false;
            int ran = Random.Range(0, GameMgr.cardBuffer.Count);
            for(int j = 0; j < GameMgr.cardInBagList.Count; j++)
            {
                if(ran == GameMgr.cardInBagList[j])
                {
                    Debug.Log("�ִ�");
                    isOverlap = true;
                    break;
                }

                for(int k = 0; k < i; k++)
                {
                    if(ran == newCardNums[k])
                    {
                        Debug.Log("�ִ�");
                        isOverlap = true;
                        break;
                    }
                }
            }
            if(!isOverlap)
            {
                newCardNums[i] = ran;
                i++;
            }
        }

        for(int i = 0; i < newCardNums.Length; i++)
        {
            Debug.Log(newCardNums[i]);
        }
    }

    void PopNewCards()
    {
        for(int i = 0; i < newCardNums.Length;)
        {
            for (int j = 0; j < GameMgr.cardBuffer.Count; j++)
            {
                if (GameMgr.cardBuffer[j].cardNum == newCardNums[i])
                {
                    GameObject card = Instantiate(cardPrefab);
                    card.transform.SetParent(newCardList.transform);
                    CardMgr cardInfo = card.GetComponent<CardMgr>();
                    cardInfo.SetCard(GameMgr.cardBuffer[j]);
                    i++;
                    break;
                }
                else
                {
                    continue;
                }
            }
        }
    }
}
