using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterInfoPanel : MonoBehaviour
{
    public Text MonsterName;
    public Text MonsterInfo;

    public void ClosePanel()
    {
        Destroy(gameObject);
    }
}
