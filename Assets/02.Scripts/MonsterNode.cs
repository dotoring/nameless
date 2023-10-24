using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterNode : MonoBehaviour
{
    public MonsterCtrl monster;

    public Text monHPText;
    public Text monDmgText;

    Animator animator;
    FieldMgr fieldMgr;

    void Awake()
    {
        //animator = GetComponent<Animator>();

        if(this.name.Contains("NomalSlime"))
            monster = new Slime();
        else if(this.name.Contains("MageSlime"))
            monster = new MageSlime();
        else if (this.name.Contains("SpikeSlime"))
            monster = new SpikeSlime();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(monster.name);
        monster.monHPText = monHPText;
        monster.monDmgText = monDmgText;
        monster.RefreshMonStat();
        monster.monsterObject = this.gameObject;

        GameObject fieldObj = GameObject.Find("FieldMgr");
        fieldMgr = fieldObj.GetComponent<FieldMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        if(monster.isMove)
        {
            StartCoroutine(MonMoveAnim(monster.isEnemyOnTile));
            monster.isMove = false;
        }
    }

    IEnumerator MonMoveAnim(bool isEnemyOnTile)
    {
        //animator.SetBool("IsAct", true);
        if (isEnemyOnTile)
        {
            while (transform.position != fieldMgr.field[monster.monPosX, monster.monPosY].transform.position + new Vector3(0.5f, 0f, 0f))
            {
                transform.position = Vector2.MoveTowards(this.transform.position, fieldMgr.field[monster.monPosX, monster.monPosY].transform.position + new Vector3(0.5f, 0f, 0f), 3 * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            while (transform.position != fieldMgr.field[monster.monPosX, monster.monPosY].transform.position)
            {
                transform.position = Vector2.MoveTowards(this.transform.position, fieldMgr.field[monster.monPosX, monster.monPosY].transform.position, 3 * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }
        }
        //animator.SetBool("IsAct", false);
        yield break;
    }
}
