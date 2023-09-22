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

    public void ObjOnTile(int posX, int posY, GameObject obj)   //플레이어,몬스터가 움직였을 떄 해당 칸에 지정해주는 함수
    {
        TileMgr tile = field[posX, posY].GetComponent<TileMgr>();
        if(obj != null)
        {
            if (obj.tag == "Player")    //obj가 플레이어라면 playerObj에 플레이어 넣기
            {
                tile.playerObj = obj;
            }
            else if (obj.tag == "Monster")   //obj가 몬스터라면 monsterObj에 몬스터 넣기
            {
                tile.monsterObj = obj;
            }
        }
    }

    public bool IsPlayerOnTile(int posX, int posY)  //해당 타일에 플레이어 존재 여부 확인 함수
    {
        TileMgr tile = field[posX, posY].GetComponent<TileMgr>();
        if (tile.playerObj != null)
        {
            return true;
        }
        return false;
    }

    public bool IsMonOnTile(int posX, int posY)     //해당 타일에 몬스터 존재 여부 확인 함수
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
