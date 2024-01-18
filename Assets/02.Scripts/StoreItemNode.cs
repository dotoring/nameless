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
    public Button itemInfoBtn = null;
    public GameObject itemInfoPanel = null;
    public Text itemInfoName = null;
    public Text itemInfo = null;

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

        itemInfoName.text = itemTemp.itemName;
        itemInfo.text = itemTemp.itemInfo;
    }

    void BuyItem()
    {
        if(StatusUIMgr.UseGold(itemCost) && StatusUIMgr.ItemCount() < 3)
        {
            StatusUIMgr.AddItem(itemNum);
            buyBtn.gameObject.SetActive(false);
            if(itemImg != null)
            {
                itemImg.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }

    private void OnMouseEnter()
    {
        itemInfoPanel.SetActive(true);
    }

    private void OnMouseExit()
    {
        itemInfoPanel.SetActive(false);
    }
}
