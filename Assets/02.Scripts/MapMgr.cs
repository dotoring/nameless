using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapMgr : MonoBehaviour
{
    public Button stage1 = null;

    // Start is called before the first frame update
    void Start()
    {
        GameMgr.RefreshHP();
        GameMgr.RefreshSP();

        if(stage1 != null)
        {
            stage1.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("BattleScene");
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
