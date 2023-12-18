using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningSlime : MonsterCtrl
{
    public LightningSlime()
    {
        mType = MonsterType.melee;
        name = "���� ������";
        maxMonHp = 10;
        monHP = maxMonHp;
        monPosX = 5;
        monPosY = 2;
        monDmg = 15;
        monInfo = "���ڴ�� �����̸�\n������ ����Ų��";
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
        isAttack = true;
        AreaOnOff(monPosX + moveToX, monPosY + moveToY, CharAction.attack, true);
    }

    public override void MonsterAction()
    {
        AreaOnOff(monPosX + moveToX, monPosY + moveToY, CharAction.attack, false);
        MonAttack(monPosX + moveToX, monPosY + moveToY, monDmg);
        Move(moveToX, moveToY);
        playerCtrl.Move(0, 0);
    }

    public override void MonActionSelect()
    {
        monsterAction = CharAction.attack;
    }

    public override void monAttackAI()
    {
        Lightning();
    }

    //������ �������� �����̰� �ϴ� �Լ�
    void Lightning()
    {
        moveToX = 0;
        moveToY = 0;
        List<int> ints = new List<int>() { 0, 1, 2, 3 }; //�������� ������ ���� ����Ʈ

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        for(int i = 0; i < dx.Length; i++)
        {
            int index = Random.Range(0, ints.Count);

            int vx = monPosX + dx[ints[index]];
            int vy = monPosY + dy[ints[index]];

            if (IsPath(vx, vy))
            {
                //IsPath���� �Ÿ��� ���� �÷��̾�� ���Ͱ� ���� �ִ� ������ �̵��Ϸ� �� ��� ����
                if(fieldMgr.IsMonOnTile(vx, vy) && fieldMgr.IsPlayerOnTile(vx, vy))
                {
                    ints.RemoveAt(index);
                    continue;
                }
                moveToX = dx[ints[index]];
                moveToY = dy[ints[index]];
                break;
            }
            ints.RemoveAt(index);
        }
    }
}
