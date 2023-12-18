using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BossKingSlime : MonsterCtrl
{
    int skill;
    int count;
    public BossKingSlime()
    {
        mType = MonsterType.melee;
        name = "킹슬라임";
        maxMonHp = 50;
        monHP = maxMonHp;
        monPosX = 5;
        monPosY = 2;
        monDmg = 15;
        monInfo = "슬라임의 왕";
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
            isAttack = true;
            switch (skill) {
                case 0:
                    AreaOnOff(monPosX, monPosY, CharAction.attack, true);
                    break;
                case 1:
                    AreaOnOff(monPosX + 1, monPosY + 1, CharAction.attack, true);
                    AreaOnOff(monPosX + 1, monPosY + 0, CharAction.attack, true);
                    AreaOnOff(monPosX + 1, monPosY - 1, CharAction.attack, true);
                    AreaOnOff(monPosX + 0, monPosY + 1, CharAction.attack, true);
                    AreaOnOff(monPosX + 0, monPosY - 1, CharAction.attack, true);
                    AreaOnOff(monPosX - 1, monPosY + 1, CharAction.attack, true);
                    AreaOnOff(monPosX - 1, monPosY + 0, CharAction.attack, true);
                    AreaOnOff(monPosX - 1, monPosY - 1, CharAction.attack, true);
                    break;
                case 2:
                    AreaOnOff(monPosX-1 - count, monPosY, CharAction.attack, true);
                    AreaOnOff(monPosX-1 - count, monPosY-1, CharAction.attack, true);
                    AreaOnOff(monPosX-1 - count, monPosY+1, CharAction.attack, true);
                    break;
            }
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
            switch (skill)
            {
                case 0:
                    AreaOnOff(monPosX, monPosY, CharAction.attack, false);
                    MonAttack(monPosX, monPosY, 40);
                    break;
                case 1:
                    AreaOnOff(monPosX + 1, monPosY + 1, CharAction.attack, false);
                    AreaOnOff(monPosX + 1, monPosY + 0, CharAction.attack, false);
                    AreaOnOff(monPosX + 1, monPosY - 1, CharAction.attack, false);
                    AreaOnOff(monPosX + 0, monPosY + 1, CharAction.attack, false);
                    AreaOnOff(monPosX + 0, monPosY - 1, CharAction.attack, false);
                    AreaOnOff(monPosX - 1, monPosY + 1, CharAction.attack, false);
                    AreaOnOff(monPosX - 1, monPosY + 0, CharAction.attack, false);
                    AreaOnOff(monPosX - 1, monPosY - 1, CharAction.attack, false);
                    MonAttack(monPosX + 1, monPosY + 1, monDmg);
                    MonAttack(monPosX + 1, monPosY + 0, monDmg);
                    MonAttack(monPosX + 1, monPosY - 1, monDmg);
                    MonAttack(monPosX + 0, monPosY + 1, monDmg);
                    MonAttack(monPosX + 0, monPosY - 1, monDmg);
                    MonAttack(monPosX - 1, monPosY + 1, monDmg);
                    MonAttack(monPosX - 1, monPosY + 0, monDmg);
                    MonAttack(monPosX - 1, monPosY - 1, monDmg);
                    break;
                case 2:
                    AreaOnOff(monPosX - 1 - count, monPosY + 1, CharAction.attack, false);
                    AreaOnOff(monPosX - 1 - count, monPosY, CharAction.attack, false);
                    AreaOnOff(monPosX - 1 - count, monPosY - 1, CharAction.attack, false);
                    MonAttack(monPosX - 1 - count, monPosY + 1, 30);
                    MonAttack(monPosX - 1 - count, monPosY, 30);
                    MonAttack(monPosX - 1 - count, monPosY - 1, 30);
                    break;
            }
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

        if(skill == 2 && count < 2)
        {
            count++;
        }
        else
        {
            if (playerCoords.x == monPosX && playerCoords.y == monPosY)
            {
                monsterAction = CharAction.attack;
                skill = 0;
            }
            else if (Math.Abs(playerCoords.x - monPosX) <= 1 && Math.Abs(playerCoords.y - monPosY) <= 1)
            {
                monsterAction = CharAction.attack;
                skill = 1;
            }
            else
            {
                System.Random rand = new System.Random();
                int r = rand.Next(10);
                if (r % 2 == 0)
                {
                    monsterAction = CharAction.attack;
                    skill = 2;
                    count = 0;
                }
                else
                {
                    monsterAction = CharAction.move;
                }
            }
        }
    }

    public override void monMoveAI()
    {
        base.monMoveAI();
    }
}
