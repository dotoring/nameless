using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonsterCtrl
{
    public Slime()
    {
        mType = MonsterType.melee;
        name = "slime";
        maxMonHp = 15;
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
