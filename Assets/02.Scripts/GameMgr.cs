using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    //�÷��̾� ���ú���
    public static int maxHp = 100;
    public static int curHp;
    public static int maxSp = 100;
    public static int curSp;

    //���� ī�帮��Ʈ
    public static List<int> cardInBagList = new List<int>(new int[] {0, 1, 2, 3, 4});

    [SerializeField] CardSO cardSO = null;
    public static List<Card> cardBuffer = new List<Card>();

    public static Text hpTxt;
    public static Text spTxt;

    public static GameMgr Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60; //���� ������ �ӵ� 60���������� ����
        QualitySettings.vSyncCount = 0; //����� �ֻ��� ����

        //GameObject hptext = GameObject.Find("HpTxt");
        //hpTxt = hptext.GetComponent<Text>();
        //GameObject sptext = GameObject.Find("SpTxt");
        //spTxt = sptext.GetComponent<Text>();

        SetUpCardBuffer();

        cardInBagList.Add(5);
        cardInBagList.Add(6);
        //cardInBagList.Add(9);

        curHp = maxHp;
        curSp = maxSp;
        RefreshHP();
        RefreshSP();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetUpCardBuffer()
    {
        for(int i = 0; i < cardSO.cardList.Length; i++)
        {
            cardBuffer.Add(cardSO.cardList[i]);
        }

    }

    public static void RefreshHP()
    {
        GameObject hptext = GameObject.Find("HpTxt");
        hpTxt = hptext.GetComponent<Text>();
        hpTxt.text = "HP " + curHp;
    }

    public static void RefreshSP()
    {
        GameObject sptext = GameObject.Find("SpTxt");
        spTxt = sptext.GetComponent<Text>();
        spTxt.text = "SP " + curSp;
    }
}
