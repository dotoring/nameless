using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapMgr : MonoBehaviour
{
    public GameObject cardBagContent = null;
    public GameObject cardPrefab = null;

    public Button bagBtn = null;
    public Button bagCloseBtn = null;
    public GameObject bag = null;

    public GameObject map = null;
    public Button test = null;

    // Start is called before the first frame update
    void Start()
    {
        BattleMgr.phase = Phase.map;

        //���� ���� �������� ��ġ��
        map.transform.position = new Vector3(((GameMgr.stage - 1) * -250), 720, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
