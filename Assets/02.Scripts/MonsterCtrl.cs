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
    public MonsterType mType;
    public string name;
    public int maxMonHp;
    public int monHP;
    public Text monHPText;
    public int monDmg;
    public Text monDmgText;

    public int monPosX = 0;
    public int monPosY = 0;

    public int moveToX;
    public int moveToY;

    public bool isEnemyOnTile;
    public bool isMove = false;

    public CharAction monsterAction;
    public GameObject monsterObject;

    //-------���� ���� ����--------
    public int reflect = 0;

    //-------���� �̵� ����----------
    Queue<Node> queue = new Queue<Node>(); //BFS�˰��򿡼� ����� ť
    bool[,] visit = new bool[7, 5]; //Ž�� �Ѱ� üũ ����


    //�÷��̾� ��ġ ����
    public Coords playerCoords;

    FieldMgr fieldMgr = null;
    public PlayerCtrl playerCtrl = null;

    // Start is called before the first frame update
    public MonsterCtrl()
    {
        mType = MonsterType.melee;
        maxMonHp = 15;
        monHP = maxMonHp;
        monPosX = 2;
        monPosY = 0;

        GameObject playerObj = GameObject.FindWithTag("Player");
        playerCtrl = playerObj.GetComponent<PlayerCtrl>();

        monsterAction = CharAction.util;

        playerCoords = playerCtrl.GetPlayerCoords();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MonsterSpawnPoint(GameObject gameObject)
    {
        GameObject fieldObj = GameObject.Find("FieldMgr");
        fieldMgr = fieldObj.GetComponent<FieldMgr>();

        if (fieldMgr != null)   //���� ù ��ġ ����
        {
            Debug.Log(monPosX + " " + monPosY);
            gameObject.transform.position = fieldMgr.field[monPosX, monPosY].transform.position;
            fieldMgr.ObjOnTile(monPosX, monPosY, gameObject);
        }
    }

    public virtual void MonsterActionArea() //���� �׼� ĭ ǥ�� �Լ�
    {
        if(monsterAction == CharAction.attack)
        {
            AreaOnOff(playerCoords.x, playerCoords.y, CharAction.attack, true);
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

    public virtual void MonsterAction() //���� �׼� ���� �Լ�
    {
        if (monsterAction == CharAction.attack)
        {
            AreaOnOff(playerCoords.x, playerCoords.y, CharAction.attack, false);
            MonAttack(playerCoords.x, playerCoords.y, monDmg);
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

    public void AreaOnOff(int x, int y, CharAction action, bool b)  //��ȣ�ۿ� ĭ ǥ�� on/off
    {
        if(action == CharAction.move)   //�̵��Ҷ�
        {
            TileMgr tile = fieldMgr.field[monPosX + x, monPosY + y].GetComponent<TileMgr>();
            tile.transform.Find("MoveArea").gameObject.SetActive(b);
        }
        else if(action == CharAction.attack) //�����ϋ�
        {
            if(x < FieldMgr.fieldWidth && x >= 0 &&
               y < FieldMgr.fieldHeight && y >= 0)
            {
                TileMgr tile = fieldMgr.field[x, y].GetComponent<TileMgr>();
                tile.transform.Find("AttArea").gameObject.SetActive(b);
            }
        }
        else //��ƿ�϶�
        {
            TileMgr tile = fieldMgr.field[x, y].GetComponent<TileMgr>();
            tile.transform.Find("UtilArea").gameObject.SetActive(b);
        }
    }

    public void Move(int x, int y)  //���� �̵��Լ�
    {
        fieldMgr.ClearObjOnTile(monPosX, monPosY, false);   //���� ��ġ ���� ����

        monPosX += x;
        monPosY += y;

        fieldMgr.ObjOnTile(monPosX, monPosY, monsterObject);   //�̵��� ��ġ�� ���� �����ֱ�
        isEnemyOnTile = fieldMgr.IsPlayerOnTile(monPosX, monPosY); //�̵��� ��ġ�� �÷��̾� ���� ���� Ȯ��
        isMove = true;

    }

    public void MonAttack(int x, int y, int dmg)    //���� �����Լ�
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

    public void MonDamage(int dmg)  //���Ͱ� ���ظ� �޴� �Լ�
    {
        monHP -= dmg;
        RefreshMonStat();
        if(monHP <= 0)  //���� ü���� 0���ϰ� �Ǹ� ���ó��
        {
            GameObject obj = GameObject.Find("BattleMgr");
            BattleMgr battleMgr = obj.GetComponent<BattleMgr>();
            battleMgr.monsters.Remove(monsterObject);  //���� ����Ʈ���� ���� ���� ����
            Destroy(monsterObject);
        }
    }

    public void RefreshMonStat()
    {
        monHPText.text = monHP.ToString();
        monDmgText.text = monDmg.ToString();
    }

    public virtual void MonActionSelect()  //���� ���� AI
    {
        playerCoords = playerCtrl.GetPlayerCoords();    //�÷��̾� ��ġ ��������

        if(monsterAction == CharAction.attack)  //������ �� �ൿ�� �����̿��ٸ�
        {
            monsterAction = CharAction.util;    //��ƿ ���� ������ֱ�
        }
        else //�� �ൿ�� ������ �ƴϿ��ٸ�
        {
            //----�÷��̾ ���� ���� �ȿ� ������ ����
            if (Mathf.Abs(monPosX - playerCoords.x) + Mathf.Abs(monPosY - playerCoords.y) <= 1)
            {
                monsterAction = CharAction.attack;
            }
            //----�ƴϸ� �̵�
            else
            {
                monsterAction = CharAction.move;
            }
        }
    }

    public virtual void monMoveAI()
    {
        playerCoords = playerCtrl.GetPlayerCoords();    //�÷��̾� ��ġ ��������

        //BFS ��ã�� �˰����� �̿��� �÷��̾ ���� �̵�
        Node monsterPos = new Node(monPosX, monPosY, null, 0);
        BFS(monsterPos);
    }

    public virtual void monAttackAI()
    {

    }

    void BFS(Node s)    //�ʺ�켱Ž��(Breadth-First Search) �˰���
    {
        visit = new bool[7, 5];

        //���Ͽ��� ���ʷ� Ȯ������ ����
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        queue.Enqueue(s);   //������ġ ť�� �ֱ�

        while (queue.Count > 0) //ť�� ��Ұ� ���� ������
        {
            Node node = queue.Dequeue();    //ť���� node�ϳ� ��������
            if (node.x == playerCoords.x && node.y == playerCoords.y)   //�÷��̾��� ��ġ�� ã���� ��
            {
                Node p = node;
                while (p.dis != 1)  //ù��°�� �̵��ߴ� ������ ã�ư���
                {
                    p = p.prev;
                }
                moveToX = p.x - monPosX;    //ù��°�� �̵��ߴ� ����� �̵���ġ ����
                moveToY = p.y - monPosY;
                queue.Clear();  //ť �ʱ�ȭ �� ������
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
    public bool IsPath(int x, int y) //�̵� ������ ������ üũ �Լ�
    {
        if (x < 0 || x > FieldMgr.fieldWidth - 1 || y < 0 || y > FieldMgr.fieldHeight - 1)  //�� ���� ��
        {
            return false;
        }

        if (visit[x, y] || (fieldMgr.IsMonOnTile(x, y) && !fieldMgr.IsPlayerOnTile(x, y)))  //Ž���� ���̰ų� ���Ͱ� �ִ� ���� �� �÷��̾ ������ ok
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
