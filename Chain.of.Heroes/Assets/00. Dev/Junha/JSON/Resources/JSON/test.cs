using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class test : MonoBehaviour
{
    public int id;

    PlayerStatus status;
    void Start()
    {
        status = JSONReader.playerDic[id];
        Debug.Log(status.Level);
    }

    void Update()
    {
        debug();
    }

    private void debug()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            ++id;
            status = JSONReader.playerDic[id];
            Debug.Log(status.Level);

            Debug.Log("·¹º§ ¾÷!, " + status.Level);

            status.debugInfo();
        }
    }
}