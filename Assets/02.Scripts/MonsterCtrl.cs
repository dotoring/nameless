using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterType
{
    melee,
    ranger,
    bomber,
    boss
}

public class MonsterCtrl : MonoBehaviour
{
    [SerializeField] MonsterSO monsterSO = null;
    public MonsterType mType;
    public int monHP;
    public Text monHPText;

    public int monPosX;
    public int monPosY;

    int moveToX;
    int moveToY;

    public CharAction monsterAction;

    //-------몬스터 이동 변수----------
    Queue<Node> queue = new Queue<Node>(); //BFS알고리즘에서 사용할 큐
    bool[,] visit = new bool[7, 5]; //탐색 한곳 체크 변수


    //플레이어 위치 변수
    Coords playerCoords;

    FieldMgr fieldMgr = null;
    PlayerCtrl playerCtrl = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject fieldObj = GameObject.Find("FieldMgr");
        fieldMgr = fieldObj.GetComponent<FieldMgr>();
        GameObject playerObj = GameObject.FindWithTag("Player");
        playerCtrl = playerObj.GetComponent<PlayerCtrl>();

        monsterAction = CharAction.util;

        playerCoords = playerCtrl.GetPlayerCoords();

        SetMonster();
        RefreshMonHP();

        if (fieldObj != null )   //몬스터 첫 위치 지정
        {
            transform.position = fieldMgr.field[monPosX, monPosY].transform.position;
            fieldMgr.ObjOnTile(monPosX, monPosY, gameObject);
        }

        //Debug.Log(fieldMgr.ObjOnTile(monPosX,monPosY,null));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void SetMonster()
    {
        Monster temp = monsterSO.monsters[1];

        monHP = temp.hp;
        monHPText.text = monHP.ToString();
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = temp.monsterImg;
    }

    public void MonsterActionArea() //몬스터 액션 칸 표시 함수
    {
        if(monsterAction == CharAction.attack)
        {
            if(mType == MonsterType.melee)
            {
                AreaOnOff(playerCoords.x, playerCoords.y, CharAction.attack, true);
            }
            else if(mType == MonsterType.ranger)
            {
                AreaOnOff(monPosX-1, monPosY, CharAction.attack, true);
                AreaOnOff(monPosX-2, monPosY, CharAction.attack, true);
                AreaOnOff(monPosX-3, monPosY, CharAction.attack, true);
                AreaOnOff(monPosX-4, monPosY, CharAction.attack, true);
            }
        }
        else if(monsterAction == CharAction.util)
        {
            AreaOnOff(monPosX, monPosY, CharAction.util, true);
        }
        else if(monsterAction == CharAction.move)
        {
            AreaOnOff(moveToX, moveToY, CharAction.move, true);
        }
    }

    public void MonsterAction() //몬스터 액션 수행 함수
    {
        if (monsterAction == CharAction.attack)
        {
            if (mType == MonsterType.melee)
            {
                AreaOnOff(playerCoords.x, playerCoords.y, CharAction.attack, false);
                MonAttack(playerCoords.x, playerCoords.y, 20);
            }
            else if (mType == MonsterType.ranger)
            {
                AreaOnOff(monPosX - 1, monPosY, CharAction.attack, false);
                AreaOnOff(monPosX - 2, monPosY, CharAction.attack, false);
                AreaOnOff(monPosX - 3, monPosY, CharAction.attack, false);
                AreaOnOff(monPosX - 4, monPosY, CharAction.attack, false);
                MonAttack(monPosX-1, monPosY, 10);
                MonAttack(monPosX-2, monPosY, 10);
                MonAttack(monPosX-3, monPosY, 10);
                MonAttack(monPosX-4, monPosY, 10);
            }
        }
        else if(monsterAction == CharAction.util)
        {
            AreaOnOff(monPosX, monPosY, CharAction.util, false);
        }
        else if (monsterAction == CharAction.move)
        {
            AreaOnOff(moveToX, moveToY, CharAction.move, false);
            Move(moveToX, moveToY);
        }
    }

    public void AreaOnOff(int x, int y, CharAction action, bool b)  //상호작용 칸 표시 on/off
    {
        if(action == CharAction.move)   //이동할때
        {
            TileMgr tile = fieldMgr.field[monPosX + x, monPosY + y].GetComponent<TileMgr>();
            tile.transform.Find("MoveArea").gameObject.SetActive(b);
        }
        else if(action == CharAction.attack) //공격일떄
        {
            if(x < FieldMgr.fieldWidth && x >= 0 &&
               y < FieldMgr.fieldHeight && y >= 0)
            {
                TileMgr tile = fieldMgr.field[x, y].GetComponent<TileMgr>();
                tile.transform.Find("AttArea").gameObject.SetActive(b);
            }
        }
        else //유틸일때
        {
            TileMgr tile = fieldMgr.field[x, y].GetComponent<TileMgr>();
            tile.transform.Find("UtilArea").gameObject.SetActive(b);
        }
    }

    public void Move(int x, int y)  //몬스터 이동함수
    {
        fieldMgr.ClearObjOnTile(monPosX, monPosY, false);   //기존 위치 정보 삭제

        monPosX += x;
        monPosY += y;

        fieldMgr.ObjOnTile(monPosX, monPosY, gameObject);   //이동할 위치에 몬스터 정보넣기
        bool isEnemyOnTile = fieldMgr.IsPlayerOnTile(monPosX, monPosY); //이동할 위치에 플레이어 존재 여부 확인
        if (isEnemyOnTile)  //적이 있다면 같이 있을 수 있게 위치 조정해주기
        {
            transform.position = fieldMgr.field[monPosX, monPosY].transform.position;
            transform.Translate(0.5f, 0, 0);
        }
        else //적이 없다면 이동할 위치로 옮겨주기
        {
            transform.position = fieldMgr.field[monPosX, monPosY].transform.position;
        }
    }

    public void MonAttack(int x, int y, int dmg)    //몬스터 공격함수
    {
        if (x < FieldMgr.fieldWidth && x >= 0 &&
            y < FieldMgr.fieldHeight && y >= 0)
        {
            TileMgr tile = fieldMgr.field[x, y].GetComponent<TileMgr>();

            if (tile.playerObj != null)
            {
                playerCtrl.PlayerDamage(dmg);
            }
        }
    }

    public void MonDamage(int dmg)  //몬스터가 피해를 받는 함수
    {
        monHP -= dmg;
        RefreshMonHP();
        if(monHP <= 0)  //몬스터 체력이 0이하가 되면 사망처리
        {
            GameObject obj = GameObject.Find("BattleMgr");
            BattleMgr battleMgr = obj.GetComponent<BattleMgr>();
            battleMgr.monsters.Remove(gameObject);  //몬스터 리스트에서 죽은 몬스터 제거
            Destroy(gameObject);
        }
    }

    void RefreshMonHP()
    {
        monHPText.text = monHP.ToString();
    }

    public void MonActionSelect()  //몬스터 동작 AI
    {
        playerCoords = playerCtrl.GetPlayerCoords();    //플레이어 위치 가져오기

        if(monsterAction == CharAction.attack)  //몬스터의 전 행동이 공격이였다면
        {
            monsterAction = CharAction.util;    //유틸 상태 만들어주기
        }
        else //전 행동이 공격이 아니였다면
        {
            if (mType == MonsterType.melee)  //근접 몬스터의 경우
            {
                //----플레이어가 공격 범위에 있으면 공격
                //근접 몬스터의 공격 범위
                if (Mathf.Abs(monPosX - playerCoords.x) + Mathf.Abs(monPosY - playerCoords.y) <= 1)
                {
                    monsterAction = CharAction.attack;
                }
                //----아니면 이동
                else
                {
                    monsterAction = CharAction.move;
                }
            }
            else if (mType == MonsterType.ranger)
            {
                //----플레이어가 공격 범위에 있으면 공격
                //원거리 몬스터의 공격 범위
                if (monPosY == playerCoords.y)
                {
                    monsterAction = CharAction.attack;
                }
                //----아니면 이동
                else
                {
                    monsterAction = CharAction.move;
                }
            }
        }
    }

    public void monMoveAI()
    {
        if (mType == MonsterType.melee) //근접 몬스터의 경우
        {
            //BFS 길찾기 알고리즘을 이용해 플레이어를 향해 이동
            Node monsterPos = new Node(monPosX, monPosY, null, 0);
            BFS(monsterPos);
        }
        else if(mType == MonsterType.ranger)    //원거리 몬스터의 경우
        {
            if (monPosY <= playerCoords.y)
            {
                //if(fieldMgr.IsMonOnTile(monPosX, monPosY +1))   //
                //{
                //    monPosY - 
                //}
                monPosY += 1;
            }
            else if (monPosY >= playerCoords.y)
            {
                monPosY -= 1;
            }
        }
    }

    void BFS(Node s)    //너비우선탐색(Breadth-First Search) 알고리즘
    {
        visit = new bool[7, 5];

        //상하우좌 차례로 확인해줄 변수
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        queue.Enqueue(s);   //시작위치 큐에 넣기

        while (queue.Count > 0) //큐에 요소가 없을 때까지
        {
            Node node = queue.Dequeue();    //큐에서 node하나 가져오기
            if (node.x == playerCoords.x && node.y == playerCoords.y)   //플레이어의 위치를 찾았을 떄
            {
                Node p = node;
                while (p.dis != 1)  //첫번째로 이동했던 노드까지 찾아가기
                {
                    p = p.prev;
                }
                moveToX = p.x - monPosX;    //첫번째로 이동했던 노드의 이동위치 설정
                moveToY = p.y - monPosY;
                queue.Clear();  //큐 초기화 후 끝내기
                break;
            }

            for (int i = 0; i < dx.Length; i++)
            {
                int vx = node.x + dx[i];
                int vy = node.y + dy[i];

                if (IsPath(vx, vy))
                {
                    visit[vx, vy] = true;
                    queue.Enqueue(new Node(vx, vy, node, node.dis + 1));
                }
            }
        }


    }
    public bool IsPath(int x, int y) //이동 가능한 길인지 체크 함수
    {
        if (x < 0 || x > FieldMgr.fieldWidth - 1 || y < 0 || y > FieldMgr.fieldHeight - 1)  //맵 밖일 때
        {
            return false;
        }

        if (visit[x, y] || fieldMgr.IsMonOnTile(x, y))  //탐색한 곳이거나 몬스터가 있는 곳일 때
        {
            return false;
        }

        return true;
    }


    class Node
    {
        public int x;
        public int y;
        public Node prev;
        public int dis;

        public Node(int x, int y, Node prev, int dis)
        {
            this.x = x;
            this.y = y;
            this.prev = prev;
            this.dis = dis;
        }
    }
}
