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

        GameObject playerObj = Instantiate(playerPrefab);   //플레이어 생성
        playerCtrl = playerObj.GetComponent<PlayerCtrl>();

        totalMonsterCount = Random.Range(1, 3);
        for (int i = 0; i < totalMonsterCount; i++)  //몬스터들 생성
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

        phase = Phase.cardSelect;   //페이즈 설정

        //가방에 있는 카드만 동적생성
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

        //--------개발자용------------
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
        //--------개발자용------------

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
                getGoldText.text = "획득골드 : " + earnGold;
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

            //선택된 카드의 카드매니져 가져오기
            CardMgr cardMgr = selectedCardList[i].GetComponent<CardMgr>();

            //몬스터 행동 결정해주기
            if(monsters.Count > 0)
            {
                for(int monster = 0;  monster < monsters.Count; monster++)
                {
                    MonsterCtrl monCtrl = monsters[monster].GetComponent<MonsterCtrl>();
                    monCtrl.MonActionSelect();  //플레이어의 위치에 따라 행동 결정
                }
            }

            //=====================1.이동 || 유틸=====================
            //----------플레이어
            if (selectedCardList[i].tag == "MoveCard")
            {
                //선택된 카드오브젝트들 하나씩 지우기
                Transform[] childList = selectedCardArea.GetComponentsInChildren<Transform>();
                Destroy(childList[1].gameObject);

                playerCtrl.MoveAreaOnOff(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y, true);   //이동 위치 표시
                yield return new WaitForSeconds(1.0f);  //이동표시 후 1초 쉬기
                playerCtrl.MoveAreaOnOff(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y, false);  //이동 위치 표시 끄기
                playerCtrl.Move(cardMgr.cardCoords[0].x, cardMgr.cardCoords[0].y);  //위치로 이동
                for (int j = 0; j < monsters.Count; j++)
                {
                    MonsterCtrl monCtrl = monsters[j].GetComponent<MonsterCtrl>();
                    monCtrl.Move(0, 0);
                }
                yield return new WaitForSeconds(1.0f);  //플레이어 행동 후 1초 쉬기
            }
            else if (selectedCardList[i].tag == "UtilCard")
            {
                //선택된 카드오브젝트들 하나씩 지우기
                Transform[] childList = selectedCardArea.GetComponentsInChildren<Transform>();
                Destroy(childList[1].gameObject);

                playerCtrl.UtilAreaOnOff(true);
                yield return new WaitForSeconds(1.0f);  //공격표시 후 1초 쉬기
                playerCtrl.UtilAreaOnOff(false);
                //GameMgr.curSp += cardMgr.cardSP;
                yield return new WaitForSeconds(1.0f);  //플레이어 행동 후 1초 쉬기

            }

            //----------몬스터
            if (monsters.Count > 0) //몬스터가 있다면
            {
                for (int j = 0; j < monsters.Count; j++)    //모든 몬스터 순서대로
                {
                    MonsterCtrl monCtrl = monsters[j].GetComponent<MonsterCtrl>();
                    //몬스터 행동이 이동이라면
                    if (monCtrl.monsterAction == CharAction.move)
                    {
                        monCtrl.monMoveAI();
                        monCtrl.MonsterActionArea();
                        yield return new WaitForSeconds(1.0f);  //몬스터 행동 표시 후 1초 쉬기
                        monCtrl.MonsterAction();
                        playerCtrl.Move(0, 0);  //위치 재조정을 위해
                        yield return new WaitForSeconds(1.0f);  //몬스터 행동 후 1초 쉬기
                    }
                    //몬스터 행동이 유틸이라면
                    else if (monCtrl.monsterAction == CharAction.util)
                    {
                        monCtrl.MonsterActionArea();
                        yield return new WaitForSeconds(1.0f);  //몬스터 행동 표시 후 1초 쉬기
                        monCtrl.MonsterAction();
                        yield return new WaitForSeconds(1.0f);  //몬스터 행동 후 1초 쉬기
                    }
                }
            }

            //=====================3.공격=====================
            //----------플레이어
            if (selectedCardList[i].tag == "AttCard")
            {
                //선택된 카드오브젝트들 하나씩 지우기
                Transform[] childList = selectedCardArea.GetComponentsInChildren<Transform>();
                Destroy(childList[1].gameObject);

                for (int j = 0; j < cardMgr.cardCoords.Count; j++)   //모든 공격 위치 1초간 표시
                {
                    playerCtrl.AttackAreaOnOff(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, true);
                }

                yield return new WaitForSeconds(1.0f);  //공격표시 후 1초 쉬기

                for (int j = 0; j < cardMgr.cardCoords.Count; j++)  //모든 공격 위치 표시 끄고 공격하기
                {
                    playerCtrl.PlayerAttack(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, cardMgr.cardDmg);
                    playerCtrl.AttackAreaOnOff(cardMgr.cardCoords[j].x, cardMgr.cardCoords[j].y, false);
                }
                yield return new WaitForSeconds(1.0f);  //플레이어 행동 후 1초 쉬기
            }

            //----------몬스터
            if (monsters.Count > 0) //몬스터가 있다면
            {
                for (int j = 0; j < monsters.Count; j++)    //모든 몬스터 순서대로
                {
                    MonsterCtrl monCtrl = monsters[j].GetComponent<MonsterCtrl>();
                    //몬스터 행동이 이동이라면
                    if (monCtrl.monsterAction == CharAction.attack)
                    {
                        monCtrl.MonsterActionArea();
                        yield return new WaitForSeconds(1.0f);  //몬스터 행동 표시 후 1초 쉬기
                        monCtrl.MonsterAction();
                        yield return new WaitForSeconds(1.0f);  //몬스터 행동 후 1초 쉬기
                    }
                }
            }

            //=====================4.스테이지 판정=====================
            if(monsters.Count <= 0) //몬스터가 전멸했을 경우
            {
                if (GameMgr.stage == 8) //마지막 스테이지라면 엔딩씬으로 이동
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
                selectedCardList.Clear();
                ClearSelectedCard();
                yield break;    //코루틴 종료
            }
            
            if (phase == Phase.gameOver) //플레이어가 사망했을 경우
            {
                yield break;    //코루틴 종료
            }
        }
        //------선택된 카드들 초기화해주기
        selectedCardList.Clear();
        ClearSelectedCard();
        phase = Phase.cardSelect;   //카드 선택 페이즈로 넘어가기
    }

    void continueBtnClick() //계속 버튼 클릭 시
    {
        phase = Phase.action;
        StartCoroutine(ActionPhase());
    }

    public void RefreshSelectedCardArea()   //선택된 카드 영역UI 초기화
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

    void ClearSelectedCard()    //선택된 카드들 초기화
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
        for(int i = 0; i < newCardLimit;) //등장할 수 있는 새 카드 수 만큼 반복
        {
            bool isOverlap = false;
            int ran = Random.Range(0, GameMgr.cardBuffer.Count); //카드 뽑기
            for(int j = 0; j < GameMgr.cardInBagList.Count; j++)
            {
                if(ran == GameMgr.cardInBagList[j]) //해당 카드가 이미 보유중인 경우
                {
                    isOverlap = true; //중복 플래그
                    break;
                }

                for(int k = 0; k < i; k++)
                {
                    if(ran == newCardNums[k])   //이미 뽑힌 카드라면
                    {
                        isOverlap = true; //중복 플래그
                        break;
                    }
                }
            }
            if(!isOverlap)  //중복 
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
        for(int i = 0; i < totalMonsterCount; i++) //몬스터 수 만큼
        {
            gold += Random.Range(50, 100);
        }
        //Debug.Log("계산"+gold);
        return gold;
    }
}
