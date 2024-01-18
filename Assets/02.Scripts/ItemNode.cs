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
                Debug.Log("체력포션(소)");
                StatusUIMgr.Heal(20);
                Destroy(gameObject);
                StatusUIMgr.RemoveItem(itemNum);
                break;
            case 1:
                Debug.Log("기력포션");
                StatusUIMgr.SPRecovery(20);
                Destroy(gameObject);
                StatusUIMgr.RemoveItem(itemNum);
                break;
            case 2:
                Debug.Log("체력포션(중)");
                StatusUIMgr.Heal(50);
                Destroy(gameObject);
                StatusUIMgr.RemoveItem(itemNum);
                break;
            case 3:
                Debug.Log("붕대");
                StatusUIMgr.PercentHeal(0.8f);
                Destroy(gameObject);
                StatusUIMgr.RemoveItem(itemNum);
                break;
            case 4:
                Debug.Log("체력포션(대)");
                StatusUIMgr.Heal(80);
                Destroy(gameObject);
                StatusUIMgr.RemoveItem(itemNum);
                break;
        }
    }
}
