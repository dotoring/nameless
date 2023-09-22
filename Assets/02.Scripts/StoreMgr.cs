using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoreMgr : MonoBehaviour
{
    public GameObject storeItemPanel = null;
    public GameObject storeCardPanel = null;
    public Button storeQuitBtn = null;

    public GameObject storeItemPref = null;
    public GameObject storeCardPref = null;

    List<int> storeCardList = new List<int>();
    List<int> storeItemList = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        GameMgr.RefreshHP();
        GameMgr.RefreshSP();
        GameMgr.RefreshGold();
        GameMgr.RefreshItems();

        StoreSetup();

        if (storeQuitBtn != null)
        {
            storeQuitBtn.onClick.AddListener(() =>
            {
                GameMgr.stage++;
                SceneManager.LoadScene("MapScene");
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StoreSetup()
    {
        for (int i = 0; i < 4;)  //��ü ī���� �ߺ� ���� 4�� ���� �̱�
        {
            if (GameMgr.cardBuffer.Count <= GameMgr.cardInBagList.Count + storeCardList.Count)
            {
                break;
            }
            Debug.Log(GameMgr.cardBuffer.Count - GameMgr.cardInBagList.Count - storeCardList.Count);

            int temp = Random.Range(0, GameMgr.cardBuffer.Count);
            if (storeCardList.Contains(temp) || GameMgr.cardInBagList.Contains(temp))
            {
                continue;
            }
            else
            {
                storeCardList.Add(temp);
                i++;
            }
        }

        for (int i = 0; i < storeCardList.Count; i++)   //���� ī�� ����
        {
            GameObject cardTemp = Instantiate(storeCardPref);
            cardTemp.transform.SetParent(storeCardPanel.transform, false);
            cardTemp.GetComponent<StoreCardNode>().cardNum = storeCardList[i];
        }

        for (int i = 0; i < 4;)  //��ü �������� �ߺ� ���� 4�� ���� �̱�
        {
            int temp = Random.Range(0, 5);
            if(storeItemList.Contains(temp))
            {
                continue;
            }
            else
            {
                storeItemList.Add(temp);
                i++;
            }
        }

        for (int i = 0; i < storeItemList.Count; i++)   //���� ������ ����
        {
            GameObject itemTemp = Instantiate(storeItemPref);
            itemTemp.transform.SetParent(storeItemPanel.transform, false);
            itemTemp.GetComponent<StoreItemNode>().itemNum = storeItemList[i];
        }
    }
}
