using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterNode : MonoBehaviour
{
    public MonsterCtrl monster;

    public Text monHPText;


    void Awake()
    {
        if(this.name.Contains("NomalSlime"))
            monster = new Slime();
        else if(this.name.Contains("MageSlime"))
            monster = new MageSlime();

    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(monster.name);
        monster.monHPText = monHPText;
        monster.RefreshMonHP();
        monster.monsterObject = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MonsterInfo()
    {
        Debug.Log(monster.name + " " + monster.monHP + " " + monster.monPosX + "," + monster.monPosY);
    }

    public void RefreshMonHP()
    {
        monHPText.text = monster.monHP.ToString();
    }
}
