using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterNode : MonoBehaviour
{
    public MonsterCtrl monster;

    public SpriteRenderer monSprite;
    public Text monHPText;
    public Text monDmgText;

    Animator animator;
    FieldMgr fieldMgr;
    PlayerCtrl playerCtrl;

    void Awake()
    {


        if (this.name.Contains("NomalSlime"))
            monster = new Slime();
        else if(this.name.Contains("MageSlime"))
            monster = new MageSlime();
        else if (this.name.Contains("SpikeSlime"))
            monster = new SpikeSlime();
        else if (this.name.Contains("StoneSlime"))
            monster = new StoneSlime();
        else if (this.name.Contains("LightningSlime"))
            monster = new LightningSlime();
        else if (this.name.Contains("BossKingSlime"))
            monster = new BossKingSlime();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(monster.name);
        monster.monHPText = monHPText;
        monster.monDmgText = monDmgText;
        monster.RefreshMonStat();
        monster.monsterObject = this.gameObject;

        animator = GetComponent<Animator>();

        GameObject fieldObj = GameObject.Find("FieldMgr");
        fieldMgr = fieldObj.GetComponent<FieldMgr>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        playerCtrl = playerObj.GetComponent<PlayerCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        if(monster.isMove)
        {
            StartCoroutine(MonMoving(monster.isEnemyOnTile));
            monster.isMove = false;
            Coords playerCoords = playerCtrl.GetPlayerCoords();
            if(monster.monPosX -  playerCoords.x < 0)
            {
                monSprite.flipX = false;
            }
            else
            {
                monSprite.flipX = true;
            }
        }
        
        if(monster.isAttack)
        {
            StartCoroutine(MonAttAnim(monster.isEnemyOnTile));
            monster.isAttack = false;
        }
    }

    IEnumerator MonMoving(bool isEnemyOnTile)
    {
        if (isEnemyOnTile) //움직일 위치에 플레이어가 있을 때
        {
            while (transform.position != fieldMgr.field[monster.monPosX, monster.monPosY].transform.position + new Vector3(0.5f, 0f, 0f))
            {
                transform.position = Vector2.MoveTowards(this.transform.position, fieldMgr.field[monster.monPosX, monster.monPosY].transform.position + new Vector3(0.5f, 0f, 0f), 5 * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            while (transform.position != fieldMgr.field[monster.monPosX, monster.monPosY].transform.position)
            {
                transform.position = Vector2.MoveTowards(this.transform.position, fieldMgr.field[monster.monPosX, monster.monPosY].transform.position, 5 * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }
        }
        yield break;
    }

    IEnumerator MonAttAnim(bool isEnemyOnTile)
    {
        if(!name.Contains("BossKingSlime"))
        {
            if (name.Contains("LightningSlime"))
            {
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.5f);
        }

        animator.SetTrigger("Attack");
        yield break;
    }

}
