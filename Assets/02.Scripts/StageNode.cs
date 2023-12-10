using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum StageType
{
    battle,
    store,
    rest,
}

public class StageNode : MonoBehaviour
{
    public StageType stageType;
    public Text stageText;

    // Start is called before the first frame update
    void Start()
    {
        Button button = GetComponent<Button>();

        switch(GameMgr.stage)
        {
            case 1:
                if(this.tag == "Stage1")
                    button.interactable = true;
                break;
            case 2:
                if (this.tag == "Stage2")
                    button.interactable = true;
                break;
            case 3:
                if (this.tag == "Stage3")
                    button.interactable = true;
                break;
            case 4:
                if (this.tag == "Stage4")
                    button.interactable = true;
                break;
            case 5:
                if (this.tag == "Stage5")
                    button.interactable = true;
                break;
            case 6:
                if (this.tag == "Stage6")
                    button.interactable = true;
                break;
            case 7:
                if (this.tag == "Stage7")
                    button.interactable = true;
                break;
            case 8:
                if (this.tag == "Stage8")
                    button.interactable = true;
                break;
            case 9:
                if (this.tag == "Stage9")
                    button.interactable = true;
                break;
        }
        button.onClick.AddListener(BtnClick);

        if(stageType == StageType.battle)
        {
            stageText.text = "battle";
            if(this.tag == "Stage9")
            {
                stageText.text = "boss";
            }
        }
        else if(stageType == StageType.store)
        {
            stageText.text = "store";
        }
        else if(stageType == StageType.rest)
        {
            stageText.text = "rest";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BtnClick()
    {
        if(stageType == StageType.battle)
        {
            SceneManager.LoadScene("BattleScene");
        }
        else if(stageType == StageType.store)
        {
            SceneManager.LoadScene("StoreScene");
        }
        else if (stageType == StageType.rest)
        {
            SceneManager.LoadScene("RestScene");
        }
    }
}
