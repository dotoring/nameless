using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeSlime : MonsterCtrl
{
    public SpikeSlime()
    {
        mType = MonsterType.melee;
        name = "선인장 슬라임";
        maxMonHp = 15;
        monHP = maxMonHp;
        monPosX = 5;
        monPosY = 2;
        monDmg = 10;
        reflect = 2;
        monInfo = "반사 2";
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
