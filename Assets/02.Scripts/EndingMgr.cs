using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingMgr : MonoBehaviour
{
    public Button titleBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        if(titleBtn != null)
        {
            titleBtn.onClick.AddListener(() =>
            {
                GameMgr.ResetGame();
                SceneManager.LoadScene("MapScene");
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
