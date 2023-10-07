using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class MageSlime : MonsterCtrl
{
    public MageSlime()
    {
        mType = MonsterType.ranger;
        name = "MageSlime";
        maxMonHp = 5;
        monHP = maxMonHp;
        monPosX = 5;
        monPosY = 2;

        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void MonsterActionArea()
    {
        if (monsterAction == CharAction.attack)
        {
            AreaOnOff(monPosX - 1, monPosY, CharAction.attack, true);
            AreaOnOff(monPosX - 2, monPosY, CharAction.attack, true);
            AreaOnOff(monPosX - 3, monPosY, CharAction.attack, true);
            AreaOnOff(monPosX - 4, monPosY, CharAction.attack, true);
        }
        else if (monsterAction == CharAction.util)
        {
            AreaOnOff(monPosX, monPosY, CharAction.util, true);
        }
        else if (monsterAction == CharAction.move)
        {
            AreaOnOff(moveToX, moveToY, CharAction.move, true);
        }
    }

    public override void MonsterAction()
    {
        if (monsterAction == CharAction.attack)
        {
            AreaOnOff(monPosX - 1, monPosY, CharAction.attack, false);
            AreaOnOff(monPosX - 2, monPosY, CharAction.attack, false);
            AreaOnOff(monPosX - 3, monPosY, CharAction.attack, false);
            AreaOnOff(monPosX - 4, monPosY, CharAction.attack, false);
            MonAttack(monPosX - 1, monPosY, 10);
            MonAttack(monPosX - 2, monPosY, 10);
            MonAttack(monPosX - 3, monPosY, 10);
            MonAttack(monPosX - 4, monPosY, 10);
        }
        else if (monsterAction == CharAction.util)
        {
            AreaOnOff(monPosX, monPosY, CharAction.util, false);
        }
        else if (monsterAction == CharAction.move)
        {
            AreaOnOff(moveToX, moveToY, CharAction.move, false);
            Move(moveToX, moveToY);
        }
    }

    public override void MonActionSelect()
    {
        playerCoords = playerCtrl.GetPlayerCoords();    //플레이어 위치 가져오기

        if (monsterAction == CharAction.attack)  //몬스터의 전 행동이 공격이였다면
        {
            monsterAction = CharAction.util;    //유틸 상태 만들어주기
        }
        else //전 행동이 공격이 아니였다면
        {
            //----플레이어가 공격 범위 안에 있으면 공격
            if (monPosY == playerCoords.y && monPosX-4 <= playerCoords.x)
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

    public override void monMoveAI()
    {
        playerCoords = playerCtrl.GetPlayerCoords();    //플레이어 위치 가져오기

        if (monPosY < playerCoords.y) //플레이어와 y축 위치 먼저 맞추기 막혀있으면 돌아가기
        {
            if(IsPath(monPosX, monPosY + 1))
            {
                moveToX = 0;
                moveToY = 1;
            }
            else if(IsPath(monPosX+1,monPosY))
            {
                moveToX = 1;
                moveToY = 0;
            }
            else if(IsPath(monPosX-1, monPosY))
            {
                moveToX = -1;
                moveToY = 0;
            }
            else if(IsPath(monPosX, monPosY-1))
            {
                moveToX = 0;
                moveToY = -1;
            }
        }
        else if (monPosY > playerCoords.y)
        {
            if (IsPath(monPosX, monPosY - 1))
            {
                moveToX = 0;
                moveToY = -1;
            }
            else if (IsPath(monPosX + 1, monPosY))
            {
                moveToX = 1;
                moveToY = 0;
            }
            else if (IsPath(monPosX - 1, monPosY))
            {
                moveToX = -1;
                moveToY = 0;
            }
            else if (IsPath(monPosX, monPosY + 1))
            {
                moveToX = 0;
                moveToY = 1;
            }
        }
        else if (monPosY == playerCoords.y && monPosX - 4 >= playerCoords.x)    //플레이어와 y축이 같고 사거리 밖에 있을 때
        {
            moveToX = -1;   //플레이어를 향해 이동
            moveToY = 0;
        }
        else if(monPosX == playerCoords.x && monPosY == playerCoords.y) //플레이어와 위치가 같을 때
        {
            if(IsPath(monPosX+1, monPosY))  //뒤로 이동 가능하면 이동
            {
                moveToX = 1;
                moveToY = 0;
            }
            else //불가능하면 정지
            {
                moveToX = 0;
                moveToY = 0;
            }
        }
    }
}
