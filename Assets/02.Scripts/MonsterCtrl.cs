using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCtrl : MonoBehaviour
{
    public int monHP;
    public Text monHPText;

    public int monPosX;
    public int monPosY;

    FieldMgr fieldMgr = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject fieldObj = GameObject.Find("FieldMgr");
        fieldMgr = fieldObj.GetComponent<FieldMgr>();

        monHP = 15;
        RefreshMonHP();

        if(fieldObj != null )
        {
            transform.position = fieldMgr.field[monPosX, monPosY].transform.position;
            fieldMgr.ObjOnTile(monPosX, monPosY, gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MonsterActionArea(int turn)
    {
        if (turn == 0)
        {
            MoveAreaOnOff(-1, 0, true);
        }
        else if (turn == 1)
        {
            MoveAreaOnOff(0, 1, true);
        }
        else if (turn == 2)
        {
            MoveAreaOnOff(-1, 0, true);
        }
    }
    public void MonsterAction(int turn)
    {
        if(turn == 0)
        {
            Debug.Log("Monster move (-1,0)");
            MoveAreaOnOff(-1, 0, false);
            Move(-1, 0);
        }
        else if(turn == 1)
        {
            Debug.Log("Monster move (0,1)");
            MoveAreaOnOff(0, 1, false);
            Move(0, 1);
        }
        else if(turn == 2)
        {
            Debug.Log("Monster move (-1,0)");
            MoveAreaOnOff(-1, 0, false);
            Move(-1, 0);
        }
    }

    public void MoveAreaOnOff(int x, int y, bool b)
    {
        if (monPosX + x < 0 || monPosX + x > FieldMgr.fieldWidth - 1)
        {
            x = 0;
        }
        if (monPosY + y < 0 || monPosY + y > FieldMgr.fieldHeight - 1)
        {
            y = 0;
        }
        TileMgr tile = fieldMgr.field[monPosX + x, monPosY + y].GetComponent<TileMgr>();
        tile.transform.Find("MoveArea").gameObject.SetActive(b);
    }

    public void Move(int x, int y)
    {
        fieldMgr.ClearObjOnTile(monPosX, monPosY, false);

        monPosX += x;
        monPosY += y;

        if (monPosX <= 0)
        {
            monPosX = 0;
        }
        else if (monPosX >= FieldMgr.fieldWidth - 1)
        {
            monPosX = FieldMgr.fieldWidth - 1;
        }

        if (monPosY <= 0)
        {
            monPosY = 0;
        }
        else if (monPosY >= FieldMgr.fieldHeight - 1)
        {
            monPosY = FieldMgr.fieldHeight - 1;
        }
        bool isEnemyOnTile = fieldMgr.ObjOnTile(monPosX, monPosY, gameObject);
        if (isEnemyOnTile)
        {
            transform.position = fieldMgr.field[monPosX, monPosY].transform.position;
            transform.Translate(0.5f, 0, 0);
        }
        else
        {
            transform.position = fieldMgr.field[monPosX, monPosY].transform.position;
        }
    }

    public void Damage(int dmg)
    {
        monHP -= dmg;
        RefreshMonHP();
        if(monHP <= 0)
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
}
