using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingMgr : MonoBehaviour
{
    public Button titleBtn = null;
    public Button restartBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        if(titleBtn != null)
        {
            titleBtn.onClick.AddListener(() =>
            {
                PlayerPrefs.DeleteAll();
                SceneManager.LoadScene("TitleScene");
            });
        }

        if (restartBtn != null)
        {
            restartBtn.onClick.AddListener(() =>
            {
                GlobalValue.NewGameData();
                SceneManager.LoadScene("MapScene");
                SceneManager.LoadScene("StatusUI", LoadSceneMode.Additive);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
