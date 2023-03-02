using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public int id;

    void Start()
    {
        PlayerStatus status = JSONReader.playerDic[id];
        Debug.Log(status.Level);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
