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
    //선택된 카드 순서 저장 리스트
    public static List<GameObject> selectedCardOrder = new List<GameObject>();
    //선택된 카드 저장 딕셔너리
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

        //플레이어 생성
        GameObject playerObj = Instantiate(playerPrefab);
        playerCtrl = playerObj.GetComponent<PlayerCtrl>();

        //몬스터들 생성
        MonsterSpawning();

        //페이즈 설정
        phase = Phase.cardSelect;   

        //선택된 카드 목록 초기화
        selectedCardOrder.Clear();
        selectedCardDic.Clear();

        //전투 승리시 나올 카드들 설정
        DrawRandomNewCard();
        PopNewCards();

        //===============버튼들 설정================
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

        //==============개발자용==============
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
        //==============개발자용==============
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
                getGoldText.text = "획득골드 : " + earnGold;
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

            //선택된 카드의 카드매니져 가져오기
            CardMgr cardMgr = selectedCardOrder[i].GetComponent<CardMgr>();

            //몬스터 행동 결정해주기
            if(monsters.Count > 0)
            {
                for(int monster = 0;  monster < monsters.Count; monster++)
                {
                    MonsterNode monNode = monsters[monster].GetComponent<MonsterNode>();
                    monNode.monster.MonActionSelect();  //플레이어의 위치에 따라 행동 결정
                }
            }

            turnText.text = i+1 + "Turn";
            turnImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            turnImage.gameObject.SetActive(false);

            //=====================1.이동 || 유틸=====================
            //----------플레이어
            if (selectedCardOrder[i].tag == "MoveCard")
            {
                StatusUIMgr.UseSP(cardMgr.cardSP);
                //선택된 카드오브젝트들 하나씩 지우기
                Transform[] childList = selectedCardArea.GetComponentsInChildren<Transform>();
                Destroy(childList[1].gameObject);

                playerCtrl.MoveAreaOnOff(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y, true);   //이동 위치 표시
                yield return new WaitForSeconds(0.5f);  //이동표시 후 1초 쉬기
                playerCtrl.MoveAreaOnOff(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y, false);  //이동 위치 표시 끄기
                playerCtrl.Move(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y);  //위치로 이동
                for (int j = 0; j < monsters.Count; j++)
                {
                    MonsterNode monNode = monsters[j].GetComponent<MonsterNode>();
                    monNode.monster.Move(0, 0);
                }
                yield return new WaitForSeconds(0.5f);  //플레이어 행동 후 1초 쉬기
            }
            else if (selectedCardOrder[i].tag == "UtilCard")
            {

                //선택된 카드오브젝트들 하나씩 지우기
                Transform[] childList = selectedCardArea.GetComponentsInChildren<Transform>();
                Destroy(childList[1].gameObject);

                playerCtrl.UtilAreaOnOff(true);
                yield return new WaitForSeconds(0.5f);  //유틸표시 후 1초 쉬기
                playerCtrl.UtilAreaOnOff(false);
                switch (cardMgr.utilType)
                {
                    case 0:
                        break;
                    case 1:
                        //sp 회복
                        StatusUIMgr.SPRecovery(cardMgr.cardSP);
                        break;
                    case 2:
                        StatusUIMgr.Heal(20);
                        //hp 회복
                        break;
                    case 3:
                        playerCtrl.guard = 5;
                        //방어
                        break;
                }
                yield return new WaitForSeconds(0.5f);  //플레이어 행동 후 1초 쉬기
            }

            //----------몬스터
            if (monsters.Count > 0) //몬스터가 있다면
            {
                for (int j = 0; j < monsters.Count; j++)    //모든 몬스터 순서대로
                {
                    MonsterNode monNode = monsters[j].GetComponent<MonsterNode>();
                    //몬스터 행동이 이동이라면
                    if (monNode.monster.monsterAction == CharAction.move)
                    {
                        monNode.monster.monMoveAI();
                        monNode.monster.MonsterActionArea();
                        yield return new WaitForSeconds(0.5f);  //몬스터 행동 표시 후 1초 쉬기
                        monNode.monster.MonsterAction();
                        playerCtrl.Move(0, 0);  //위치 재조정을 위해
                        yield return new WaitForSeconds(0.5f);  //몬스터 행동 후 1초 쉬기
                    }
                    //몬스터 행동이 유틸이라면
                    else if (monNode.monster.monsterAction == CharAction.util)
                    {
                        monNode.monster.MonsterActionArea();
                        yield return new WaitForSeconds(0.5f);  //몬스터 행동 표시 후 1초 쉬기
                        monNode.monster.MonsterAction();
                        yield return new WaitForSeconds(0.5f);  //몬스터 행동 후 1초 쉬기
                    }
                }
            }

            //=====================3.공격=====================
            //----------플레이어
            if (selectedCardOrder[i].tag == "AttCard")
            {
                StatusUIMgr.UseSP(cardMgr.cardSP);
                //선택된 카드오브젝트들 하나씩 지우기
                Transform[] childList = selectedCardArea.GetComponentsInChildren<Transform>();
                Destroy(childList[1].gameObject);

                for (int j = 0; j < cardMgr.cardCoords.Count; j++)   //모든 공격 위치 1초간 표시
                {
                    playerCtrl.AttackAreaOnOff(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, true);
                }

                yield return new WaitForSeconds(0.5f);  //공격표시 후 1초 쉬기

                for (int j = 0; j < cardMgr.cardCoords.Count; j++)  //모든 공격 위치 표시 끄고 공격하기
                {
                    playerCtrl.PlayerAttack(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, cardMgr.cardDmg);
                    playerCtrl.AttackAreaOnOff(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, false);
                }
                yield return new WaitForSeconds(0.8f);  //플레이어 행동 후 1초 쉬기
            }

            //----------몬스터
            if (monsters.Count > 0) //몬스터가 있다면
            {
                for (int j = 0; j < monsters.Count; j++)    //모든 몬스터 순서대로
                {
                    MonsterNode monNode = monsters[j].GetComponent<MonsterNode>();
                    //몬스터 행동이 공격이라면
                    if (monNode.monster.monsterAction == CharAction.attack)
                    {
                        monNode.monster.monAttackAI();
                        monNode.monster.MonsterActionArea();
                        yield return new WaitForSeconds(0.5f);  //몬스터 행동 표시 후 1초 쉬기
                        monNode.monster.MonsterAction();
                        yield return new WaitForSeconds(0.8f);  //몬스터 행동 후 1초 쉬기
                    }
                }
            }

            playerCtrl.guard = 0;

            //=====================4.스테이지 판정=====================
            if (monsters.Count <= 0) //몬스터가 전멸했을 경우
            {
                if (GlobalValue.stage == 9) //마지막 스테이지라면 엔딩씬으로 이동
                {
                    SceneManager.LoadScene("EndingScene");
                }

                phase = Phase.battleEndNewCard; //전투 결과 페이즈로 넘어가기

                //-------선택된 카드들 전부 지워주기
                Transform[] child = selectedCardArea.GetComponentsInChildren<Transform>();
                for (int l = 1; l < child.Length; l++)
                {
                    Destroy(child[l].gameObject);
                }
                ClearSelectedCard();
                yield break;    //코루틴 종료
            }
            
            if (phase == Phase.gameOver) //플레이어가 사망했을 경우
            {
                ClearSelectedCard();
                yield break;    //코루틴 종료
            }
        }
        //------선택된 카드들 초기화해주기

        ClearSelectedCard();
        StatusUIMgr.SetTempSP();
        phase = Phase.cardSelect;   //카드 선택 페이즈로 넘어가기

        //가방버튼 활성화
        if (bagBtnObj != null)
        {
            bagBtnObj.gameObject.SetActive(true);
        }
    }

    void continueBtnClick() //계속 버튼 클릭 시
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

    public void RefreshSelectedCardArea()    //선택된 카드 영역에 카드 띄우는 함수
    {
        //원래 있던 카드 오브젝트들 모두 삭제
        Transform[] child = selectedCardArea.GetComponentsInChildren<Transform>();
        if (child != null)
        {
            for (int i = 1; i < child.Length; i++)
            {
                Destroy(child[i].gameObject);
            }
        }

        //삭제 후 선택된 카드들 새로 넣기
        //카드 순서와 딕셔너리 비교 => 순서대로 딕셔너리의 카드 버퍼와 비교 => 버퍼의 정보로 카드 생성
        for(int i = 0; i < selectedCardOrder.Count; i++)
        {
            int order = selectedCardOrder[i].GetComponent<CardMgr>().cardKey;

            foreach (KeyValuePair<int, int> cardTemp in selectedCardDic)
            {
                if(cardTemp.Key == order) //순서 찾기
                {
                    for (int j = 0; j < GameMgr.cardBuffer.Count; j++)
                    {
                        if (GameMgr.cardBuffer[j].cardNum == cardTemp.Value) //해당 카드 버퍼에서 찾기
                        {
                            //카드 생성
                            GameObject card = Instantiate(selectedCardPrefab);
                            card.transform.SetParent(selectedCardArea.transform, false);
                            SelectedCardNode cardInfo = card.GetComponent<SelectedCardNode>();
                            cardInfo.SetCard(GameMgr.cardBuffer[j], cardTemp.Key);
                            break; //카드 생성 후 다음 카드로
                        }
                    }
                    break; //다음 순서로
                }
            }
        }
    }

    public void ClearSelectedCard()    //선택된 카드들 초기화
    {
        selectedCardOrder.Clear();  //순서 리스트 초기화
        selectedCardDic.Clear();    //딕셔너리 초기화

        statusUIMgr.ClearIsSelected();

        //가방 안의 카드들 isSelected 초기화
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
        for(int i = 0; i < newCardLimit;) //등장할 수 있는 새 카드 수 만큼 반복
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

    void PopNewCards() //새 카드 띄워주는 함수
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

    int GoldCalc()  //획득 골드 계산 함수
    {
        int gold = 0;
        for(int i = 0; i < totalMonsterCount; i++) //몬스터 수 만큼
        {
            gold += Random.Range(50, 100);
        }
        return gold;
    }

    void MonsterSpawning()
    {
        if(GlobalValue.stage == 1) //첫 스테이지
        {
            GameObject mon = Instantiate(monsterPrefabs[0]);
            monsters.Add(mon);

            MonsterNode monNode = monsters[0].GetComponent<MonsterNode>();
            monNode.monster.monPosX = 5;
            monNode.monster.monPosY = 2;
            monNode.monster.MonsterSpawnPoint(monsters[0]);
        }
        else if (GlobalValue.stage <= 3) //두마리 스폰
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
        else if (GlobalValue.stage <= 6) //세마리 스폰
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
        else if(GlobalValue.stage == 9) //보스 스테이지
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
