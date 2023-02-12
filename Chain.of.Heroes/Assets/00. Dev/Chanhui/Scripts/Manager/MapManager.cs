using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance = null;
    // 인스턴스에 접근하기 위한 프로퍼티
    public static MapManager Instance
    {
        get
        {
            // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(MapManager)) as MapManager;

                if (_instance == null)
                {
                    Debug.Log("no Singleton obj");
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this) // 인스턴스가 존재하는 경우 새로생기는 인스턴스를 삭제
        {
            Destroy(gameObject);
        }
    }

    public MapData[] monData;
    public int stageNum = 0;

}
