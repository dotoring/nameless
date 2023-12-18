using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSlime : MonsterCtrl
{
    public StoneSlime()
    {
        mType = MonsterType.melee;
        name = "암석 슬라임";
        maxMonHp = 20;
        monHP = maxMonHp;
        monPosX = 5;
        monPosY = 2;
        monDmg = 10;
        block = 3;
        monInfo = "방어 3";
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
        base.MonsterActionArea();
    }

    public override void MonsterAction()
    {
        base.MonsterAction();
    }

    public override void MonActionSelect()
    {
        base.MonActionSelect();
    }

    public override void monMoveAI()
    {
        base.monMoveAI();
    }
}
