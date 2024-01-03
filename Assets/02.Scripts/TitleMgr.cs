using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMgr : MonoBehaviour
{
    public Button newGameBtn = null;
    public Button continueGameBtn = null;
    public Button quitBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60; //실행 프레임 속도 60프레임으로 고정
        QualitySettings.vSyncCount = 0; //모니터 주사율 고정

        if (newGameBtn != null)
        {
            newGameBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MapScene");
                //SceneManager.LoadScene("StatusUI", LoadSceneMode.Additive);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
