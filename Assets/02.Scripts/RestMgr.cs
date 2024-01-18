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
        StatusUIMgr.Heal(30);
        GlobalValue.stage++;
        SceneManager.LoadScene("MapScene");
        SceneManager.LoadScene("StatusUI", LoadSceneMode.Additive);
    }

    void HealthBtnClick()
    {
        StatusUIMgr.IncreaseHP(10);
        GlobalValue.stage++;
        SceneManager.LoadScene("MapScene");
        SceneManager.LoadScene("StatusUI", LoadSceneMode.Additive);
    }

    void MeditBtnClick()
    {
        StatusUIMgr.IncreaseSP(10);
        GlobalValue.stage++;
        SceneManager.LoadScene("MapScene");
        SceneManager.LoadScene("StatusUI", LoadSceneMode.Additive);
    }
}
