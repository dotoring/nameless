using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNode : MonoBehaviour
{
    public int itemNum = 0;
    public Image itemImage;
    Button itemBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        SetItem();
        itemBtn = GetComponent<Button>();
        itemBtn.onClick.AddListener(ItemUse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetItem()
    {
        Item itemTemp = GameMgr.itemBuffer[itemNum];
        itemImage.sprite = itemTemp.itemImage;
    }

    void ItemUse()
    {
        switch(itemNum)
        {
            case 0:
                Debug.Log("체력포션");
                GameMgr.curHp += 10;
                GameMgr.RefreshHP();
                Destroy(gameObject);
                GameMgr.itemList.Remove(itemNum);
                break;
            case 1:
                Debug.Log("기력포션");
                GameMgr.curSp += 10;
                GameMgr.RefreshSP();
                Destroy(gameObject);
                GameMgr.itemList.Remove(itemNum);
                break;
            case 2:
                Debug.Log("힘포션");
                Destroy(gameObject);
                GameMgr.itemList.Remove(itemNum);
                break;
            case 3:
                Debug.Log("붕대");
                GameMgr.curHp = GameMgr.maxHp * 0.8f;
                GameMgr.RefreshHP();
                Destroy(gameObject);
                GameMgr.itemList.Remove(itemNum);
                break;
            case 4:
                Debug.Log("진통제");
                Destroy(gameObject);
                GameMgr.itemList.Remove(itemNum);
                break;
        }
    }
}
