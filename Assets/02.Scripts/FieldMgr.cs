using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldMgr : MonoBehaviour
{
    public static int fieldWidth = 7;
    public static int fieldHeight = 5;
    public GameObject tile;

    public GameObject[,] field = new GameObject[fieldWidth, fieldHeight];

    private void Awake()
    {

        for (int i = 0; i < fieldWidth; i++)
        {
            for (int j = 0; j < fieldHeight; j++)
            {
                field[i, j] = Instantiate(tile);

                field[i, j].transform.position = new Vector3(i * 2, j);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ObjOnTile(int posX, int posY, GameObject obj)   //�÷��̾�,���Ͱ� �������� �� �ش� ĭ�� �������ִ� �Լ�
    {
        TileMgr tile = field[posX, posY].GetComponent<TileMgr>();
        if(obj != null)
        {
            if (obj.tag == "Player")    //obj�� �÷��̾��� playerObj�� �÷��̾� �ֱ�
            {
                tile.playerObj = obj;
            }
            else if (obj.tag == "Monster")   //obj�� ���Ͷ�� monsterObj�� ���� �ֱ�
            {
                tile.monsterObj = obj;
            }
        }
    }

    public bool IsPlayerOnTile(int posX, int posY)  //�ش� Ÿ�Ͽ� �÷��̾� ���� ���� Ȯ�� �Լ�
    {
        TileMgr tile = field[posX, posY].GetComponent<TileMgr>();
        if (tile.playerObj != null)
        {
            return true;
        }
        return false;
    }

    public bool IsMonOnTile(int posX, int posY)     //�ش� Ÿ�Ͽ� ���� ���� ���� Ȯ�� �Լ�
    {
        TileMgr tile = field[posX, posY].GetComponent<TileMgr>();
        if (tile.monsterObj != null)
        {
            return true;
        }
        return false;
    }

    public void ClearObjOnTile(int posX, int posY, bool isPlayer)
    {
        TileMgr tile = field[posX, posY].GetComponent<TileMgr>();

        if (isPlayer)
        {
            tile.playerObj = null;
        }
        else
        {
            tile.monsterObj = null;
        }
    }
}
