using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneClick : MonoBehaviour
{
    public void SceneChange()
    {
        string nowbutton = EventSystem.current.currentSelectedGameObject.name;
        switch (nowbutton)
        {
            case "Stage1":
                MapManager.Instance.stageNum = 0;
                break;
            case "Stage2":
                MapManager.Instance.stageNum = 1;
                break;
        }

        SceneManager.LoadScene("Chan");
    }
}
