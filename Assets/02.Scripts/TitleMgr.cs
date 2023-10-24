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
        if(newGameBtn != null)
        {
            newGameBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MapScene");
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
