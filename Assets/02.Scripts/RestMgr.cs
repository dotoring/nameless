using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestMgr : MonoBehaviour
{
    public Button restBtn;
    public Button healthBtn;
    public Button meditBtn;
    
    // Start is called before the first frame update
    void Start()
    {
        if (restBtn != null)
        {
            restBtn.onClick.AddListener(RestBtnClick);
        }

        if (healthBtn != null)
        {
            healthBtn.onClick.AddListener(HealthBtnClick);
        }

        if (meditBtn != null)
        {
            meditBtn.onClick.AddListener(MeditBtnClick);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RestBtnClick()
    {
        GameMgr.curHp += 30;
        if(GameMgr.curHp >= GameMgr.maxHp)
        {
            GameMgr.curHp = GameMgr.maxHp;
        }
        GameMgr.stage++;
        SceneManager.LoadScene("MapScene");
        SceneManager.LoadScene("StatusUI", LoadSceneMode.Additive);
    }

    void HealthBtnClick()
    {
        GameMgr.maxHp += 10;
        GameMgr.stage++;
        SceneManager.LoadScene("MapScene");
        SceneManager.LoadScene("StatusUI", LoadSceneMode.Additive);
    }

    void MeditBtnClick()
    {
        GameMgr.maxSp += 10;
        GameMgr.curSp = GameMgr.maxSp;
        GameMgr.stage++;
        SceneManager.LoadScene("MapScene");
        SceneManager.LoadScene("StatusUI", LoadSceneMode.Additive);
    }
}
