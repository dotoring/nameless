using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningSlime : MonsterCtrl
{
    public LightningSlime()
    {
        mType = MonsterType.melee;
        name = "번개 슬라임";
        maxMonHp = 10;
        monHP = maxMonHp;
        monPosX = 5;
        monPosY = 2;
        monDmg = 15;
        monInfo = "제멋대로 움직이며\n번개를 일으킨다";
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

    //무작위 방향으로 움직이게 하는 함수
    void Lightning()
    {
        moveToX = 0;
        moveToY = 0;
        List<int> ints = new List<int>() { 0, 1, 2, 3 }; //동서남북 랜덤을 위한 리스트

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        for(int i = 0; i < dx.Length; i++)
        {
            int index = Random.Range(0, ints.Count);

            int vx = monPosX + dx[ints[index]];
            int vy = monPosY + dy[ints[index]];

            if (IsPath(vx, vy))
            {
                //IsPath에서 거르지 못한 플레이어와 몬스터가 같이 있는 곳으로 이동하려 할 경우 차단
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
