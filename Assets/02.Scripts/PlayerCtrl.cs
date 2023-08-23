using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerCtrl : MonoBehaviour
{
    FieldMgr fieldMgr = null;

    int playerPosX = 1;
    int playerPosY = 2;

    // Start is called before the first frame update
    void Start()
    {
        GameObject fieldObj = GameObject.Find("FieldMgr");
        fieldMgr = fieldObj.GetComponent<FieldMgr>();

        if (fieldMgr != null)
        {
            transform.position = fieldMgr.field[playerPosX, playerPosY].transform.position;
            fieldMgr.ObjOnTile(playerPosX, playerPosY, gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveAreaOnOff(int x, int y, bool b)
    {
        if(playerPosX + x < 0 || playerPosX + x > FieldMgr.fieldWidth - 1)
        {
            x = 0;
        }
        if(playerPosY + y < 0 || playerPosY + y> FieldMgr.fieldHeight - 1)
        {
            y = 0;
        }
        TileMgr tile = fieldMgr.field[playerPosX + x, playerPosY + y].GetComponent<TileMgr>();
        tile.transform.Find("MoveArea").gameObject.SetActive(b);
    }

    public void Move(int x, int y)
    {
        fieldMgr.ClearObjOnTile(playerPosX, playerPosY, true);

        playerPosX += x;
        playerPosY += y;
        
        if(playerPosX <= 0)
        {
            playerPosX = 0;
        }
        else if (playerPosX >= FieldMgr.fieldWidth - 1)
        {
            playerPosX = FieldMgr.fieldWidth - 1;
        }

        if (playerPosY <= 0)
        {
            playerPosY = 0;
        }
        else if(playerPosY >= FieldMgr.fieldHeight - 1)
        {
            playerPosY = FieldMgr.fieldHeight - 1;
        }
        bool isEnemyOnTile = fieldMgr.ObjOnTile(playerPosX, playerPosY, gameObject);
        if (isEnemyOnTile)
        {
            transform.position = fieldMgr.field[playerPosX, playerPosY].transform.position;
            transform.Translate(-0.5f, 0, 0);
        }
        else
        {
            transform.position = fieldMgr.field[playerPosX, playerPosY].transform.position;
        }
    }

    public void AttackAreaOnOff(int x, int y, bool b)
    {
        TileMgr tile = fieldMgr.field[playerPosX + x, playerPosY + y].GetComponent<TileMgr>();
        tile.transform.Find("AttArea").gameObject.SetActive(b);
    }

    public void Attack(int x, int y, int dmg)
    {
        TileMgr tile = fieldMgr.field[playerPosX + x, playerPosY + y].GetComponent<TileMgr>();

        if (tile.monsterObj != null)
        {
            tile.monsterObj.GetComponent<MonsterCtrl>().Damage(dmg);
        }
    }
}
