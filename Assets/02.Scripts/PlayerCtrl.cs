using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public Animator animator = null;
    bool isPlayerMove = false;

    // Start is called before the first frame update
    void Start()
    {
        //animator = gameObject.GetComponent<Animator>();

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
        StartCoroutine(MoveAnim(isEnemyOnTile));
    }

    IEnumerator MoveAnim(bool isEnemyOnTile)
    {
        animator.SetBool("IsMove", true);
        if (isEnemyOnTile)
        {
            while (transform.position != fieldMgr.field[playerPosX, playerPosY].transform.position - new Vector3(0.5f, 0f, 0f))
            {
                transform.position = Vector2.MoveTowards(this.transform.position, fieldMgr.field[playerPosX, playerPosY].transform.position - new Vector3(0.5f, 0f, 0f), 3 * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            while (transform.position != fieldMgr.field[playerPosX, playerPosY].transform.position)
            {
                transform.position = Vector2.MoveTowards(this.transform.position, fieldMgr.field[playerPosX, playerPosY].transform.position, 3 * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }
        }
        animator.SetBool("IsMove", false);
        yield break;
    }

    public void AttackAreaOnOff(int x, int y, bool b)
    {
        if (b)
        {
            StartCoroutine(AttackAnim());
        }
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

    IEnumerator AttackAnim()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("IsAttack", true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("IsAttack", false);
        yield break;
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
