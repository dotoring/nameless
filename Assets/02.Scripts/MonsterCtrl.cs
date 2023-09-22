using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCtrl : MonoBehaviour
{
    public int monHP;
    public Text monHPText;

    public int monPosX;
    public int monPosY;

    int moveToX;
    int moveToY;

    bool canMonsterAttack = false;
    bool monsterUtil = false;

    //-------���� �̵� ����----------
    Queue<Node> queue = new Queue<Node>(); //BFS�˰��򿡼� ����� ť

    //�÷��̾� ��ġ ����
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

        playerCoords = playerCtrl.GetPlayerCoords();

        monHP = 15;
        RefreshMonHP();

        if (fieldObj != null )   //���� ù ��ġ ����
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

    public void MonsterActionArea() //���� �׼� ĭ ǥ�� �Լ�
    {
        playerCoords = playerCtrl.GetPlayerCoords();

        MonActionAI();

        if(canMonsterAttack == true)
        {
            AreaOnOff(playerCoords.x, playerCoords.y, CharAction.attack, true);
        }
        else if(monsterUtil == true)
        {
            AreaOnOff(monPosX, monPosY, CharAction.util, true);
        }
        else
        {
            AreaOnOff(moveToX, moveToY, CharAction.move, true);
        }
    }

    public void MonsterAction() //���� �׼� ���� �Լ�
    {
        if (canMonsterAttack == true)
        {
            AreaOnOff(playerCoords.x, playerCoords.y, CharAction.attack, false);
            MonAttack(playerCoords.x, playerCoords.y, 20);
        }
        else if(monsterUtil == true)
        {
            AreaOnOff(monPosX, monPosY, CharAction.util, false);
        }
        else
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
            TileMgr tile = fieldMgr.field[x, y].GetComponent<TileMgr>();
            tile.transform.Find("AttArea").gameObject.SetActive(b);
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

        fieldMgr.ObjOnTile(monPosX, monPosY, gameObject);   //�̵��� ��ġ�� ���� �����ֱ�
        bool isEnemyOnTile = fieldMgr.IsPlayerOnTile(monPosX, monPosY); //�̵��� ��ġ�� �÷��̾� ���� ���� Ȯ��
        if (isEnemyOnTile)  //���� �ִٸ� ���� ���� �� �ְ� ��ġ �������ֱ�
        {
            transform.position = fieldMgr.field[monPosX, monPosY].transform.position;
            transform.Translate(0.5f, 0, 0);
        }
        else //���� ���ٸ� �̵��� ��ġ�� �Ű��ֱ�
        {
            transform.position = fieldMgr.field[monPosX, monPosY].transform.position;
        }
    }

    public void MonAttack(int x, int y, int dmg)    //���� �����Լ�
    {
        TileMgr tile = fieldMgr.field[x, y].GetComponent<TileMgr>();

        if (tile.playerObj != null)
        {
            playerCtrl.PlayerDamage(dmg);
        }
    }

    public void MonDamage(int dmg)  //���Ͱ� ���ظ� �޴� �Լ�
    {
        monHP -= dmg;
        RefreshMonHP();
        if(monHP <= 0)  //���� ü���� 0���ϰ� �Ǹ� ���ó��
        {
            GameObject obj = GameObject.Find("BattleMgr");
            BattleMgr battleMgr = obj.GetComponent<BattleMgr>();
            battleMgr.monsters.Remove(gameObject);  //���� ����Ʈ���� ���� ���� ����
            Destroy(gameObject);
        }
    }

    void RefreshMonHP()
    {
        monHPText.text = monHP.ToString();
    }

    void MonActionAI()  //���� ���� AI
    {
        monsterUtil = false;
        if(canMonsterAttack)
        {
            canMonsterAttack = false;
            monsterUtil = true;
            return;
        }

        //----�÷��̾ �տ� ������ ����
        if (Mathf.Abs(monPosX - playerCoords.x) + Mathf.Abs(monPosY - playerCoords.y) <= 1)
        {
            canMonsterAttack = true;
        }
        //----�ƴϸ� �÷��̾ ���� �̵�
        else
        {
            //BFS ��ã�� �˰����� �̿�
            Node monsterPos = new Node(monPosX, monPosY, null, 0);
            BFS(monsterPos);
        }
    }

    void BFS(Node s)    //�ʺ�켱Ž��(Breadth-First Search) �˰���
    {
        bool[,] visit = new bool[7, 5]; //Ž�� �Ѱ� üũ ����

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

        bool IsPath(int x, int y) //�̵� ������ ������ üũ �Լ�
        {
            if (x < 0 || x > FieldMgr.fieldWidth - 1 || y < 0 || y > FieldMgr.fieldHeight - 1)  //�� ���� ��
            {
                return false;
            }

            if (visit[x, y] || fieldMgr.IsMonOnTile(x, y))  //Ž���� ���̰ų� ���Ͱ� �ִ� ���� ��
            {
                return false;
            }

            return true;
        }
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
