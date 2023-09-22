using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemNode : MonoBehaviour
{
    public Image itemImg = null;
    public Text itemName = null;
    public int itemNum = 0;
    public int itemCost = 0;
    public Text itemCostTxt = null;
    public Button buyBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        SetStoreItem();

        if(buyBtn != null)
        {
            buyBtn.onClick.AddListener(BuyItem);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetStoreItem()
    {
        Item itemTemp = GameMgr.itemBuffer[itemNum];

        itemImg.sprite = itemTemp.itemImage;
        itemName.text = itemTemp.itemName;
        itemCost = itemTemp.itemCost;
        itemCostTxt.text = itemCost.ToString();
    }

    void BuyItem()
    {
        if(GameMgr.curGold >= itemCost && GameMgr.itemList.Count < 3)
        {
            GameMgr.curGold -= itemCost;
            GameMgr.RefreshGold();
            GameMgr.itemList.Add(itemNum);
            GameMgr.RefreshItems();
            buyBtn.gameObject.SetActive(false);
            if(itemImg != null)
            {
                Debug.Log("test");
                itemImg.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }
}
