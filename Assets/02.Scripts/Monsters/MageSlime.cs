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
        playerCoords = playerCtrl.GetPlayerCoords();    //�÷��̾� ��ġ ��������

        if (monsterAction == CharAction.attack)  //������ �� �ൿ�� �����̿��ٸ�
        {
            monsterAction = CharAction.util;    //��ƿ ���� ������ֱ�
        }
        else //�� �ൿ�� ������ �ƴϿ��ٸ�
        {
            //----�÷��̾ ���� ���� �ȿ� ������ ����
            if (monPosY == playerCoords.y && monPosX-4 <= playerCoords.x)
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

    public override void monMoveAI()
    {
        playerCoords = playerCtrl.GetPlayerCoords();    //�÷��̾� ��ġ ��������

        if (monPosY < playerCoords.y) //�÷��̾�� y�� ��ġ ���� ���߱� ���������� ���ư���
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
        else if (monPosY == playerCoords.y && monPosX - 4 >= playerCoords.x)    //�÷��̾�� y���� ���� ��Ÿ� �ۿ� ���� ��
        {
            moveToX = -1;   //�÷��̾ ���� �̵�
            moveToY = 0;
        }
        else if(monPosX == playerCoords.x && monPosY == playerCoords.y) //�÷��̾�� ��ġ�� ���� ��
        {
            if(IsPath(monPosX+1, monPosY))  //�ڷ� �̵� �����ϸ� �̵�
            {
                moveToX = 1;
                moveToY = 0;
            }
            else //�Ұ����ϸ� ����
            {
                moveToX = 0;
                moveToY = 0;
            }
        }
    }
}
