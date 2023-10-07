using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum CharAction
{
    attack,
    move,
    util,
}

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
        fieldMgr.ObjOnTile(playerPosX, playerPosY, gameObject); //이동할 위치에 플레이어 정보넣기
        bool isEnemyOnTile = fieldMgr.IsMonOnTile(playerPosX, playerPosY);  //이동할 위치에 몬스터 존재 여부 확인
        if (isEnemyOnTile)  //적이 있다면 같이 있을 수 있게 위치 조정해주기
        {
            transform.position = fieldMgr.field[playerPosX, playerPosY].transform.position;
            transform.Translate(-0.5f, 0, 0);
        }
        else //적이 없다면 이동할 위치로 옮겨주기
        {
            transform.position = fieldMgr.field[playerPosX, playerPosY].transform.position;
        }
    }

    public void AttackAreaOnOff(int x, int y, bool b)
    {
        if (playerPosX + x < FieldMgr.fieldWidth && playerPosX + x >= 0 &&
            playerPosY + y < FieldMgr.fieldHeight && playerPosY + y >= 0)
        {
            TileMgr tile = fieldMgr.field[playerPosX + x, playerPosY + y].GetComponent<TileMgr>();
            if (tile != null)
            {
                tile.transform.Find("AttArea").gameObject.SetActive(b);
            }
        }
    }

    public void PlayerAttack(int x, int y, int dmg)
    {
        if (playerPosX + x < FieldMgr.fieldWidth && playerPosX + x >= 0 &&
            playerPosY + y < FieldMgr.fieldHeight && playerPosY + y >= 0)
        {
            TileMgr tile = fieldMgr.field[playerPosX + x, playerPosY + y].GetComponent<TileMgr>();
            if (tile != null && tile.monsterObj != null)
            {
                tile.monsterObj.GetComponent<MonsterNode>().monster.MonDamage(dmg);
            }
        }
    }

    public void UtilAreaOnOff(bool b)
    {
        TileMgr tile = fieldMgr.field[playerPosX, playerPosY].GetComponent<TileMgr>();
        tile.transform.Find("UtilArea").gameObject.SetActive(b);
    }

    public void PlayerDamage(int dmg)
    {
        GameMgr.curHp -= dmg;
        GameMgr.RefreshHP();
        if (GameMgr.curHp <= 0)
        {
            BattleMgr.phase = Phase.gameOver;
            Debug.Log("game over");
        }
    }

    public Coords GetPlayerCoords()
    {
        Coords playerCoords = new Coords();
        playerCoords.x = playerPosX;
        playerCoords.y = playerPosY;
        return playerCoords;
    }
}
