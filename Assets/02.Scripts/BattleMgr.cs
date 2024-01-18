using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    public StatusUIMgr statusUIMgr = null;
    GameObject bagBtnObj = null;

    public GameObject cardPrefab = null;
    public GameObject selectedCardPrefab = null;

    public GameObject selectedCardArea = null;
    //���õ� ī�� ���� ���� ����Ʈ
    public static List<GameObject> selectedCardOrder = new List<GameObject>();
    //���õ� ī�� ���� ��ųʸ�
    public static Dictionary<int, int> selectedCardDic = new Dictionary<int, int>();

    public Button continueBtn = null;

    public GameObject[] monsterPrefabs = null;
    public GameObject playerPrefab = null;
    PlayerCtrl playerCtrl = null;

    public Image turnImage = null;
    public Text turnText = null;

    [Header("------Result------")]
    public GameObject resultPanel = null;
    public GameObject newCardPage = null;
    public GameObject newCardList = null;
    public GameObject resultPage = null;
    public Text getGoldText = null;
    int earnGold;
    int[] newCardNums = new int[3];
    int newCardLimit = 3;
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
    int totalMonsterCount = 5;

    // Start is called before the first frame update
    void Start()
    {
        StatusUIMgr.SetTempSP();

        GameObject statusObj = GameObject.Find("StatusUIMgr");
        statusUIMgr = statusObj.GetComponent<StatusUIMgr>();
        bagBtnObj = GameObject.Find("BagBtn");

        //�÷��̾� ����
        GameObject playerObj = Instantiate(playerPrefab);
        playerCtrl = playerObj.GetComponent<PlayerCtrl>();

        //���͵� ����
        MonsterSpawning();

        //������ ����
        phase = Phase.cardSelect;   

        //���õ� ī�� ��� �ʱ�ȭ
        selectedCardOrder.Clear();
        selectedCardDic.Clear();

        //���� �¸��� ���� ī��� ����
        DrawRandomNewCard();
        PopNewCards();

        //===============��ư�� ����================
        if (continueBtn != null)
        {
            continueBtn.onClick.AddListener(continueBtnClick);
        }

        if(nextBtn != null)
        {
            nextBtn.onClick.AddListener(() =>
            {
                StatusUIMgr.FillSP();
                StatusUIMgr.NextStage();
                SceneManager.LoadScene("MapScene");
                SceneManager.LoadScene("StatusUI", LoadSceneMode.Additive);
            });
        }

        if(newGameBtn != null)
        {
            newGameBtn.onClick.AddListener(() =>
            {
                GlobalValue.NewGameData();
                SceneManager.LoadScene("MapScene");
                SceneManager.LoadScene("StatusUI", LoadSceneMode.Additive);
            });
        }

        //==============�����ڿ�==============
        if(quitBtn != null)
        {
            quitBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("TitleScene");
            });
        }

        if(windBtn != null)
        {
            windBtn.onClick.AddListener(() =>
            {
                phase = Phase.battleEndNewCard;
            });
        }
        //==============�����ڿ�==============
    }

    // Update is called once per frame
    void Update()
    {
        if(phase == Phase.action)
        {
            continueBtn.gameObject.SetActive(false);
        }
        else if(phase == Phase.cardSelect)
        {
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
                StatusUIMgr.EarnGold(earnGold);
                getGoldText.text = "ȹ���� : " + earnGold;
                isCalcDone = true;
            }
        }
        else if(phase == Phase.gameOver)
        {
            gameOverPanel.gameObject.SetActive(true);
        }

        if(selectedCardOrder.Count == 3)
        {
            continueBtn.interactable = true;
        }
        else
        {
            continueBtn.interactable = false;
        }
    }

    IEnumerator ActionPhase()
    {
        for (int i = 0; i < selectedCardOrder.Count; i++)
        {

            //���õ� ī���� ī��Ŵ��� ��������
            CardMgr cardMgr = selectedCardOrder[i].GetComponent<CardMgr>();

            //���� �ൿ �������ֱ�
            if(monsters.Count > 0)
            {
                for(int monster = 0;  monster < monsters.Count; monster++)
                {
                    MonsterNode monNode = monsters[monster].GetComponent<MonsterNode>();
                    monNode.monster.MonActionSelect();  //�÷��̾��� ��ġ�� ���� �ൿ ����
                }
            }

            turnText.text = i+1 + "Turn";
            turnImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            turnImage.gameObject.SetActive(false);

            //=====================1.�̵� || ��ƿ=====================
            //----------�÷��̾�
            if (selectedCardOrder[i].tag == "MoveCard")
            {
                StatusUIMgr.UseSP(cardMgr.cardSP);
                //���õ� ī�������Ʈ�� �ϳ��� �����
                Transform[] childList = selectedCardArea.GetComponentsInChildren<Transform>();
                Destroy(childList[1].gameObject);

                playerCtrl.MoveAreaOnOff(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y, true);   //�̵� ��ġ ǥ��
                yield return new WaitForSeconds(0.5f);  //�̵�ǥ�� �� 1�� ����
                playerCtrl.MoveAreaOnOff(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y, false);  //�̵� ��ġ ǥ�� ����
                playerCtrl.Move(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y);  //��ġ�� �̵�
                for (int j = 0; j < monsters.Count; j++)
                {
                    MonsterNode monNode = monsters[j].GetComponent<MonsterNode>();
                    monNode.monster.Move(0, 0);
                }
                yield return new WaitForSeconds(0.5f);  //�÷��̾� �ൿ �� 1�� ����
            }
            else if (selectedCardOrder[i].tag == "UtilCard")
            {

                //���õ� ī�������Ʈ�� �ϳ��� �����
                Transform[] childList = selectedCardArea.GetComponentsInChildren<Transform>();
                Destroy(childList[1].gameObject);

                playerCtrl.UtilAreaOnOff(true);
                yield return new WaitForSeconds(0.5f);  //��ƿǥ�� �� 1�� ����
                playerCtrl.UtilAreaOnOff(false);
                switch (cardMgr.utilType)
                {
                    case 0:
                        break;
                    case 1:
                        //sp ȸ��
                        StatusUIMgr.SPRecovery(cardMgr.cardSP);
                        break;
                    case 2:
                        StatusUIMgr.Heal(20);
                        //hp ȸ��
                        break;
                    case 3:
                        playerCtrl.guard = 5;
                        //���
                        break;
                }
                yield return new WaitForSeconds(0.5f);  //�÷��̾� �ൿ �� 1�� ����
            }

            //----------����
            if (monsters.Count > 0) //���Ͱ� �ִٸ�
            {
                for (int j = 0; j < monsters.Count; j++)    //��� ���� �������
                {
                    MonsterNode monNode = monsters[j].GetComponent<MonsterNode>();
                    //���� �ൿ�� �̵��̶��
                    if (monNode.monster.monsterAction == CharAction.move)
                    {
                        monNode.monster.monMoveAI();
                        monNode.monster.MonsterActionArea();
                        yield return new WaitForSeconds(0.5f);  //���� �ൿ ǥ�� �� 1�� ����
                        monNode.monster.MonsterAction();
                        playerCtrl.Move(0, 0);  //��ġ �������� ����
                        yield return new WaitForSeconds(0.5f);  //���� �ൿ �� 1�� ����
                    }
                    //���� �ൿ�� ��ƿ�̶��
                    else if (monNode.monster.monsterAction == CharAction.util)
                    {
                        monNode.monster.MonsterActionArea();
                        yield return new WaitForSeconds(0.5f);  //���� �ൿ ǥ�� �� 1�� ����
                        monNode.monster.MonsterAction();
                        yield return new WaitForSeconds(0.5f);  //���� �ൿ �� 1�� ����
                    }
                }
            }

            //=====================3.����=====================
            //----------�÷��̾�
            if (selectedCardOrder[i].tag == "AttCard")
            {
                StatusUIMgr.UseSP(cardMgr.cardSP);
                //���õ� ī�������Ʈ�� �ϳ��� �����
                Transform[] childList = selectedCardArea.GetComponentsInChildren<Transform>();
                Destroy(childList[1].gameObject);

                for (int j = 0; j < cardMgr.cardCoords.Count; j++)   //��� ���� ��ġ 1�ʰ� ǥ��
                {
                    playerCtrl.AttackAreaOnOff(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, true);
                }

                yield return new WaitForSeconds(0.5f);  //����ǥ�� �� 1�� ����

                for (int j = 0; j < cardMgr.cardCoords.Count; j++)  //��� ���� ��ġ ǥ�� ���� �����ϱ�
                {
                    playerCtrl.PlayerAttack(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, cardMgr.cardDmg);
                    playerCtrl.AttackAreaOnOff(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, false);
                }
                yield return new WaitForSeconds(0.8f);  //�÷��̾� �ൿ �� 1�� ����
            }

            //----------����
            if (monsters.Count > 0) //���Ͱ� �ִٸ�
            {
                for (int j = 0; j < monsters.Count; j++)    //��� ���� �������
                {
                    MonsterNode monNode = monsters[j].GetComponent<MonsterNode>();
                    //���� �ൿ�� �����̶��
                    if (monNode.monster.monsterAction == CharAction.attack)
                    {
                        monNode.monster.monAttackAI();
                        monNode.monster.MonsterActionArea();
                        yield return new WaitForSeconds(0.5f);  //���� �ൿ ǥ�� �� 1�� ����
                        monNode.monster.MonsterAction();
                        yield return new WaitForSeconds(0.8f);  //���� �ൿ �� 1�� ����
                    }
                }
            }

            playerCtrl.guard = 0;

            //=====================4.�������� ����=====================
            if (monsters.Count <= 0) //���Ͱ� �������� ���
            {
                if (GlobalValue.stage == 9) //������ ����������� ���������� �̵�
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
                ClearSelectedCard();
                yield break;    //�ڷ�ƾ ����
            }
            
            if (phase == Phase.gameOver) //�÷��̾ ������� ���
            {
                ClearSelectedCard();
                yield break;    //�ڷ�ƾ ����
            }
        }
        //------���õ� ī��� �ʱ�ȭ���ֱ�

        ClearSelectedCard();
        StatusUIMgr.SetTempSP();
        phase = Phase.cardSelect;   //ī�� ���� ������� �Ѿ��

        //�����ư Ȱ��ȭ
        if (bagBtnObj != null)
        {
            bagBtnObj.gameObject.SetActive(true);
        }
    }

    void continueBtnClick() //��� ��ư Ŭ�� ��
    {
        phase = Phase.action;
        GameObject bagObj = GameObject.Find("Bag");
        if (bagObj != null)
        {
            bagObj.gameObject.SetActive(false);
        }
        if (bagBtnObj != null)
        {
            bagBtnObj.gameObject.SetActive(false);
        }
        StartCoroutine(ActionPhase());
    }

    public void RefreshSelectedCardArea()    //���õ� ī�� ������ ī�� ���� �Լ�
    {
        //���� �ִ� ī�� ������Ʈ�� ��� ����
        Transform[] child = selectedCardArea.GetComponentsInChildren<Transform>();
        if (child != null)
        {
            for (int i = 1; i < child.Length; i++)
            {
                Destroy(child[i].gameObject);
            }
        }

        //���� �� ���õ� ī��� ���� �ֱ�
        //ī�� ������ ��ųʸ� �� => ������� ��ųʸ��� ī�� ���ۿ� �� => ������ ������ ī�� ����
        for(int i = 0; i < selectedCardOrder.Count; i++)
        {
            int order = selectedCardOrder[i].GetComponent<CardMgr>().cardKey;

            foreach (KeyValuePair<int, int> cardTemp in selectedCardDic)
            {
                if(cardTemp.Key == order) //���� ã��
                {
                    for (int j = 0; j < GameMgr.cardBuffer.Count; j++)
                    {
                        if (GameMgr.cardBuffer[j].cardNum == cardTemp.Value) //�ش� ī�� ���ۿ��� ã��
                        {
                            //ī�� ����
                            GameObject card = Instantiate(selectedCardPrefab);
                            card.transform.SetParent(selectedCardArea.transform, false);
                            SelectedCardNode cardInfo = card.GetComponent<SelectedCardNode>();
                            cardInfo.SetCard(GameMgr.cardBuffer[j], cardTemp.Key);
                            break; //ī�� ���� �� ���� ī���
                        }
                    }
                    break; //���� ������
                }
            }
        }
    }

    public void ClearSelectedCard()    //���õ� ī��� �ʱ�ȭ
    {
        selectedCardOrder.Clear();  //���� ����Ʈ �ʱ�ȭ
        selectedCardDic.Clear();    //��ųʸ� �ʱ�ȭ

        statusUIMgr.ClearIsSelected();

        //���� ���� ī��� isSelected �ʱ�ȭ
        //Transform[] child = cardBagContent.GetComponentsInChildren<Transform>();
        //if(child != null)
        //{
        //    for (int i = 1; i < child.Length; i++)
        //    {
        //        CardMgr cardMgr = child[i].gameObject.GetComponent<CardMgr>();
        //        if(cardMgr != null)
        //        {
        //            cardMgr.isSelected = false;
        //        }
        //    }
        //}
    }

    void DrawRandomNewCard()
    {
        for(int i = 0; i < newCardLimit;) //������ �� �ִ� �� ī�� �� ��ŭ �ݺ�
        {
            int ran = Random.Range(0, GameMgr.cardBuffer.Count);
            if (newCardNums.Contains(ran))
            {
                continue;
            }
            else
            {
                newCardNums[i] = ran;
                i++;
            }
        }
    }

    void PopNewCards() //�� ī�� ����ִ� �Լ�
    {
        for(int i = 0; i < newCardLimit; i++)
        {
            for (int j = 0; j < GameMgr.cardBuffer.Count; j++)
            {
                if (GameMgr.cardBuffer[j].cardNum == newCardNums[i])
                {
                    GameObject card = Instantiate(cardPrefab);
                    card.transform.SetParent(newCardList.transform, false);
                    CardMgr cardInfo = card.GetComponent<CardMgr>();
                    cardInfo.SetCard(GameMgr.cardBuffer[j], GlobalValue.ownCards.Count);
                    break;
                }
            }
        }
    }

    int GoldCalc()  //ȹ�� ��� ��� �Լ�
    {
        int gold = 0;
        for(int i = 0; i < totalMonsterCount; i++) //���� �� ��ŭ
        {
            gold += Random.Range(50, 100);
        }
        return gold;
    }

    void MonsterSpawning()
    {
        if(GlobalValue.stage == 1) //ù ��������
        {
            GameObject mon = Instantiate(monsterPrefabs[0]);
            monsters.Add(mon);

            MonsterNode monNode = monsters[0].GetComponent<MonsterNode>();
            monNode.monster.monPosX = 5;
            monNode.monster.monPosY = 2;
            monNode.monster.MonsterSpawnPoint(monsters[0]);
        }
        else if (GlobalValue.stage <= 3) //�θ��� ����
        {
            for (int i = 0; i < 2; i++)
            {
                int monster = Random.Range(0, 5);
                GameObject mon = Instantiate(monsterPrefabs[monster]);
                monsters.Add(mon);
            }

            for (int i = 0; i < monsters.Count; i++)
            {
                MonsterNode monNode = monsters[i].GetComponent<MonsterNode>();
                monNode.monster.monPosX = 4 + (i % 2);
                monNode.monster.monPosY = i;
                monNode.monster.MonsterSpawnPoint(monsters[i]);
            }
        }
        else if (GlobalValue.stage <= 6) //������ ����
        {
            for (int i = 0; i < 3; i++)
            {
                int monster = Random.Range(0, 5);
                GameObject mon = Instantiate(monsterPrefabs[monster]);
                monsters.Add(mon);
            }

            for (int i = 0; i < monsters.Count; i++)
            {
                MonsterNode monNode = monsters[i].GetComponent<MonsterNode>();
                monNode.monster.monPosX = 4 + (i % 2);
                monNode.monster.monPosY = i;
                monNode.monster.MonsterSpawnPoint(monsters[i]);
            }
        }
        else if(GlobalValue.stage == 9) //���� ��������
        {
            GameObject mon = Instantiate(monsterPrefabs[5]);
            monsters.Add(mon);

            MonsterNode monNode = monsters[0].GetComponent<MonsterNode>();
            monNode.monster.monPosX = 5;
            monNode.monster.monPosY = 2;
            monNode.monster.MonsterSpawnPoint(monsters[0]);
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                int monster = Random.Range(0, 5);
                GameObject mon = Instantiate(monsterPrefabs[monster]);
                monsters.Add(mon);
            }

            for (int i = 0; i < monsters.Count; i++)
            {
                MonsterNode monNode = monsters[i].GetComponent<MonsterNode>();
                monNode.monster.monPosX = 4 + (i % 2);
                monNode.monster.monPosY = i;
                monNode.monster.MonsterSpawnPoint(monsters[i]);
            }
        }

        //totalMonsterCount = 1;
        //for (int i = 0; i < totalMonsterCount; i++)
        //{
        //    GameObject mon = Instantiate(monsterPrefabs[5]);
        //    monsters.Add(mon);
        //}

        //for (int i = 0; i < monsters.Count; i++)
        //{
        //    MonsterNode monNode = monsters[i].GetComponent<MonsterNode>();
        //    monNode.monster.monPosX = 4 + (i % 2);
        //    monNode.monster.monPosY = i;
        //    monNode.monster.MonsterSpawnPoint(monsters[i]);
        //}

        //MonsterNode monNode = monsters[0].GetComponent<MonsterNode>();
        //monNode.monster.monPosX = 4;
        //monNode.monster.monPosY = 2;
        //monNode.monster.MonsterSpawnPoint(monsters[0]);

        //monNode = monsters[1].GetComponent<MonsterNode>();
        //monNode.monster.monPosX = 4;
        //monNode.monster.monPosY = 3;
        //monNode.monster.MonsterSpawnPoint(monsters[1]);

        //monNode = monsters[2].GetComponent<MonsterNode>();
        //monNode.monster.monPosX = 6;
        //monNode.monster.monPosY = 3;
        //monNode.monster.MonsterSpawnPoint(monsters[2]);

        //monNode = monsters[3].GetComponent<MonsterNode>();
        //monNode.monster.monPosX = 5;
        //monNode.monster.monPosY = 2;
        //monNode.monster.MonsterSpawnPoint(monsters[3]);

        //monNode = monsters[4].GetComponent<MonsterNode>();
        //monNode.monster.monPosX = 5;
        //monNode.monster.monPosY = 4;
        //monNode.monster.MonsterSpawnPoint(monsters[4]);
    }
}
