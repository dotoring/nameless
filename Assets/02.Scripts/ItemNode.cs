using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNode : MonoBehaviour
{
    public int itemNum = 0;
    public Image itemImage;
    Button itemBtn = null;
    public GameObject itemInfoPanel = null;
    public Text itemName = null;
    public Text itemInfo = null;
    public Button itemUseBtn = null;
    public Button itemCancelBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        SetItem();
        itemBtn = GetComponent<Button>();
        itemBtn.onClick.AddListener(() =>
        {
            GameObject panel = GameObject.Find("ItemInfoPanel");
            if (panel != null)
            {
                panel.SetActive(false);
            }
            itemInfoPanel.SetActive(!itemInfoPanel.active);
        });

        itemUseBtn.onClick.AddListener(ItemUse);

        itemCancelBtn.onClick.AddListener(() =>
        {
            itemInfoPanel.SetActive(false);
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetItem()
    {
        Item itemTemp = GameMgr.itemBuffer[itemNum];
        itemName.text = itemTemp.itemName;
        itemInfo.text = itemTemp.itemInfo;
        itemImage.sprite = itemTemp.itemImage;
    }

    void ItemUse()
    {
        switch(itemNum)
        {
            case 0:
                Debug.Log("ü������(��)");
                GameMgr.curHp += 20;
                if(GameMgr.curHp > GameMgr.maxHp)
                {
                    GameMgr.curHp = GameMgr.maxHp;
                }
                GameMgr.RefreshHP();
                Destroy(gameObject);
                GameMgr.itemList.Remove(itemNum);
                break;
            case 1:
                Debug.Log("�������");
                GameMgr.curSp += 20;
                if(GameMgr.curSp > GameMgr.maxSp)
                {
                    GameMgr.curSp = GameMgr.maxSp;
                }
                GameMgr.RefreshSP();
                Destroy(gameObject);
                GameMgr.itemList.Remove(itemNum);
                break;
            case 2:
                Debug.Log("ü������(��)");
                GameMgr.curHp += 50;
                if (GameMgr.curHp > GameMgr.maxHp)
                {
                    GameMgr.curHp = GameMgr.maxHp;
                }
                GameMgr.RefreshHP();
                Destroy(gameObject);
                GameMgr.itemList.Remove(itemNum);
                break;
            case 3:
                Debug.Log("�ش�");
                if(GameMgr.curHp < GameMgr.maxHp * 0.8f)
                {
                    GameMgr.curHp = GameMgr.maxHp * 0.8f;
                }
                GameMgr.RefreshHP();
                Destroy(gameObject);
                GameMgr.itemList.Remove(itemNum);
                break;
            case 4:
                Debug.Log("ü������(��)");
                GameMgr.curHp += 80;
                if (GameMgr.curHp > GameMgr.maxHp)
                {
                    GameMgr.curHp = GameMgr.maxHp;
                }
                GameMgr.RefreshHP();
                Destroy(gameObject);
                GameMgr.itemList.Remove(itemNum);
                break;
        }
    }
}
