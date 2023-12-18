using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonsterCtrl
{
    public Slime()
    {
        mType = MonsterType.melee;
        name = "슬라임";
        maxMonHp = 15;
        monHP = maxMonHp;
        monPosX = 5;
        monPosY = 2;
        monDmg = 15;
        monInfo = "평범한 슬라임";
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
