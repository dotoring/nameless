using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Phase
{
    cardSelect,
    action,
    battleEndNewCard,
    battleEndResult,
    gameOver,
    map
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
    public GameObject resultPanel = null;
    public GameObject newCardPage = null;
    public GameObject newCardList = null;
    public GameObject resultPage = null;
    public Text getGoldText = null;
    int earnGold;
    int[] newCardNums = new int[3];
    int newCardLimit;
    bool isCalcDone = false;

    public Button nextBtn = null;

    [Header("------GameOver------")]
    public GameObject gameOverPanel = null;
    public Text gameOverText = null;
    public Button newGameBtn = null;

    [Header("------forDev------")]
    public Button quitBtn = null;
    public Button windBtn = null;

    public static Phase phase;
    public List<GameObject> monsters = new List<GameObject>();
    int totalMonsterCount;


    // Start is called before the first frame update
    void Start()
    {
        GameMgr.RefreshHP();
        GameMgr.RefreshSP();
        GameMgr.RefreshGold();
        GameMgr.RefreshItems();

        GameObject playerObj = Instantiate(playerPrefab);   //�÷��̾� ����
        playerCtrl = playerObj.GetComponent<PlayerCtrl>();

        totalMonsterCount = Random.Range(1, 3);
        for (int i = 0; i < totalMonsterCount; i++)  //���͵� ����
        {
            GameObject mon = Instantiate(monsterPrefab);
            MonsterCtrl monCtrl = mon.GetComponent<MonsterCtrl>();
            monCtrl.monPosX = 5;
            monCtrl.monPosY = i * 2 + 1;
            monsters.Add(mon);
        }
        //GameObject mon = Instantiate(monsterPrefab);
        //MonsterCtrl monCtrl = mon.GetComponent<MonsterCtrl>();
        //monCtrl.monPosX = 5;
        //monCtrl.monPosY = 2;
        //monCtrl.mType = MonsterType.ranger;
        //monsters.Add(mon);

        phase = Phase.cardSelect;   //������ ����

        //���濡 �ִ� ī�常 ��������
        for (int i = 0; i < GameMgr.cardBuffer.Count; i++)
        {
            for(int j = 0; j < GameMgr.cardInBagList.Count; j++)
            {
                if (GameMgr.cardBuffer[i].cardNum == GameMgr.cardInBagList[j])
                {
                    GameObject card = Instantiate(cardPrefab);
                    card.transform.SetParent(cardBagContent.transform, false);
                    CardMgr cardInfo = card.GetComponent<CardMgr>();
                    cardInfo.SetCard(GameMgr.cardBuffer[i]);
                }
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
                GameMgr.stage++;
                SceneManager.LoadScene("MapScene");
            });
        }

        if(newGameBtn != null)
        {
            newGameBtn.onClick.AddListener(() =>
            {
                GameMgr.ResetGame();
                SceneManager.LoadScene("MapScene");
            });
        }

        //--------�����ڿ�------------
        if(quitBtn != null)
        {
            quitBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MapScene");
            });
        }

        if(windBtn != null)
        {
            windBtn.onClick.AddListener(() =>
            {
                phase = Phase.battleEndNewCard;
            });
        }
        //--------�����ڿ�------------

        newCardLimit = newCardNums.Length;
        if (GameMgr.cardBuffer.Count - GameMgr.cardInBagList.Count < newCardLimit)
        {
            newCardLimit = GameMgr.cardBuffer.Count - GameMgr.cardInBagList.Count;
        }

        DrawRandomNewCard();
        PopNewCards();
    }

    // Update is called once per frame
    void Update()
    {
        if(phase == Phase.action)
        {
            bag.gameObject.SetActive(false);
            bagBtn.gameObject.SetActive(false);
            continueBtn.gameObject.SetActive(false);
        }
        else if(phase == Phase.cardSelect)
        {
            bagBtn.gameObject.SetActive(true);
            continueBtn.gameObject.SetActive(true);
        }
        else if(phase == Phase.battleEndNewCard)
        {
            resultPanel.gameObject.SetActive(true);
        }
        else if(phase == Phase.battleEndResult)
        {
            newCardPage.gameObject.SetActive(false);
            resultPage.gameObject.SetActive(true);
            if(!isCalcDone)
            {
                earnGold = GoldCalc();
                GameMgr.curGold += earnGold;
                getGoldText.text = "ȹ���� : " + earnGold;
                GameMgr.RefreshGold();
                isCalcDone = true;
            }
        }
        else if(phase == Phase.gameOver)
        {
            gameOverPanel.gameObject.SetActive(true);
        }
    }

    IEnumerator ActionPhase()
    {
        for (int i = 0; i < selectedCardList.Count; i++)
        {

            //���õ� ī���� ī��Ŵ��� ��������
            CardMgr cardMgr = selectedCardList[i].GetComponent<CardMgr>();

            //���� �ൿ �������ֱ�
            if(monsters.Count > 0)
            {
                for(int monster = 0;  monster < monsters.Count; monster++)
                {
                    MonsterCtrl monCtrl = monsters[monster].GetComponent<MonsterCtrl>();
                    monCtrl.MonActionSelect();  //�÷��̾��� ��ġ�� ���� �ൿ ����
                }
            }

            //=====================1.�̵� || ��ƿ=====================
            //----------�÷��̾�
            if (selectedCardList[i].tag == "MoveCard")
            {
                //���õ� ī�������Ʈ�� �ϳ��� �����
                Transform[] childList = selectedCardArea.GetComponentsInChildren<Transform>();
                Destroy(childList[1].gameObject);

                playerCtrl.MoveAreaOnOff(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y, true);   //�̵� ��ġ ǥ��
                yield return new WaitForSeconds(1.0f);  //�̵�ǥ�� �� 1�� ����
                playerCtrl.MoveAreaOnOff(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y, false);  //�̵� ��ġ ǥ�� ����
                playerCtrl.Move(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y);  //��ġ�� �̵�
                for (int j = 0; j < monsters.Count; j++)
                {
                    MonsterCtrl monCtrl = monsters[j].GetComponent<MonsterCtrl>();
                    monCtrl.Move(0, 0);
                }
                yield return new WaitForSeconds(1.0f);  //�÷��̾� �ൿ �� 1�� ����
            }
            else if (selectedCardList[i].tag == "UtilCard")
            {
                //���õ� ī�������Ʈ�� �ϳ��� �����
                Transform[] childList = selectedCardArea.GetComponentsInChildren<Transform>();
                Destroy(childList[1].gameObject);

                playerCtrl.UtilAreaOnOff(true);
                yield return new WaitForSeconds(1.0f);  //����ǥ�� �� 1�� ����
                playerCtrl.UtilAreaOnOff(false);
                //GameMgr.curSp += cardMgr.cardSP;
                yield return new WaitForSeconds(1.0f);  //�÷��̾� �ൿ �� 1�� ����

            }

            //----------����
            if (monsters.Count > 0) //���Ͱ� �ִٸ�
            {
                for (int j = 0; j < monsters.Count; j++)    //��� ���� �������
                {
                    MonsterCtrl monCtrl = monsters[j].GetComponent<MonsterCtrl>();
                    //���� �ൿ�� �̵��̶��
                    if (monCtrl.monsterAction == CharAction.move)
                    {
                        monCtrl.monMoveAI();
                        monCtrl.MonsterActionArea();
                        yield return new WaitForSeconds(1.0f);  //���� �ൿ ǥ�� �� 1�� ����
                        monCtrl.MonsterAction();
                        playerCtrl.Move(0, 0);  //��ġ �������� ����
                        yield return new WaitForSeconds(1.0f);  //���� �ൿ �� 1�� ����
                    }
                    //���� �ൿ�� ��ƿ�̶��
                    else if (monCtrl.monsterAction == CharAction.util)
                    {
                        monCtrl.MonsterActionArea();
                        yield return new WaitForSeconds(1.0f);  //���� �ൿ ǥ�� �� 1�� ����
                        monCtrl.MonsterAction();
                        yield return new WaitForSeconds(1.0f);  //���� �ൿ �� 1�� ����
                    }
                }
            }

            //=====================3.����=====================
            //----------�÷��̾�
            if (selectedCardList[i].tag == "AttCard")
            {
                //���õ� ī�������Ʈ�� �ϳ��� �����
                Transform[] childList = selectedCardArea.GetComponentsInChildren<Transform>();
                Destroy(childList[1].gameObject);

                for (int j = 0; j < cardMgr.cardCoords.Count; j++)   //��� ���� ��ġ 1�ʰ� ǥ��
                {
                    playerCtrl.AttackAreaOnOff(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, true);
                }

                yield return new WaitForSeconds(1.0f);  //����ǥ�� �� 1�� ����

                for (int j = 0; j < cardMgr.cardCoords.Count; j++)  //��� ���� ��ġ ǥ�� ���� �����ϱ�
                {
                    playerCtrl.PlayerAttack(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, cardMgr.cardDmg);
                    playerCtrl.AttackAreaOnOff(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, false);
                }
                yield return new WaitForSeconds(1.0f);  //�÷��̾� �ൿ �� 1�� ����
            }

            //----------����
            if (monsters.Count > 0) //���Ͱ� �ִٸ�
            {
                for (int j = 0; j < monsters.Count; j++)    //��� ���� �������
                {
                    MonsterCtrl monCtrl = monsters[j].GetComponent<MonsterCtrl>();
                    //���� �ൿ�� �̵��̶��
                    if (monCtrl.monsterAction == CharAction.attack)
                    {
                        monCtrl.MonsterActionArea();
                        yield return new WaitForSeconds(1.0f);  //���� �ൿ ǥ�� �� 1�� ����
                        monCtrl.MonsterAction();
                        yield return new WaitForSeconds(1.0f);  //���� �ൿ �� 1�� ����
                    }
                }
            }

            //=====================4.�������� ����=====================
            if(monsters.Count <= 0) //���Ͱ� �������� ���
            {
                if (GameMgr.stage == 8) //������ ����������� ���������� �̵�
                {
                    SceneManager.LoadScene("EndingScene");
                }

                phase = Phase.battleEndNewCard; //���� ��� ������� �Ѿ��

                //-------���õ� ī��� ���� �����ֱ�
                Transform[] child = selectedCardArea.GetComponentsInChildren<Transform>();
                for (int l = 1; l < child.Length; l++)
                {
                    Destroy(child[l].gameObject);
                }
                selectedCardList.Clear();
                ClearSelectedCard();
                yield break;    //�ڷ�ƾ ����
            }
            
            if (phase == Phase.gameOver) //�÷��̾ ������� ���
            {
                yield break;    //�ڷ�ƾ ����
            }
        }
        //------���õ� ī��� �ʱ�ȭ���ֱ�
        selectedCardList.Clear();
        ClearSelectedCard();
        phase = Phase.cardSelect;   //ī�� ���� ������� �Ѿ��
    }

    void continueBtnClick() //��� ��ư Ŭ�� ��
    {
        phase = Phase.action;
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
            card.transform.localScale = new Vector3(0.72f, 0.72f, 1f);
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
        for(int i = 0; i < newCardLimit;) //������ �� �ִ� �� ī�� �� ��ŭ �ݺ�
        {
            bool isOverlap = false;
            int ran = Random.Range(0, GameMgr.cardBuffer.Count); //ī�� �̱�
            for(int j = 0; j < GameMgr.cardInBagList.Count; j++)
            {
                if(ran == GameMgr.cardInBagList[j]) //�ش� ī�尡 �̹� �������� ���
                {
                    isOverlap = true; //�ߺ� �÷���
                    break;
                }

                for(int k = 0; k < i; k++)
                {
                    if(ran == newCardNums[k])   //�̹� ���� ī����
                    {
                        isOverlap = true; //�ߺ� �÷���
                        break;
                    }
                }
            }
            if(!isOverlap)  //�ߺ� 
            {
                newCardNums[i] = ran;
                i++;
            }
        }
    }

    void PopNewCards()
    {

        
        for(int i = 0; i < newCardLimit;)
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

    int GoldCalc()
    {
        int gold = 0;
        for(int i = 0; i < totalMonsterCount; i++) //���� �� ��ŭ
        {
            gold += Random.Range(50, 100);
        }
        //Debug.Log("���"+gold);
        return gold;
    }
}
