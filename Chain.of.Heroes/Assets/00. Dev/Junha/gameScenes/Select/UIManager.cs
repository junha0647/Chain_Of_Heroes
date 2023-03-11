using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField, Header("Menu UI")]
    private GameObject UI_Menu;
    private bool isOnMenu = false;

    public static Sprite notSelected;
    public static Sprite Selected;

    [SerializeField, Header("ChapterInfo UI")]
    private GameObject UI_chapterInfo;

    private void Start()
    {
        StartCoroutine(LoadSprites());
    }

    private IEnumerator LoadSprites()
    {
        // AssetBundle 로드
        string bundleUrl = "file://" + Application.dataPath + "/00. Dev/Junha/AssetBundles";
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl);
        yield return www.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

        // 스프라이트 로드
        notSelected = bundle.LoadAsset<Sprite>("Not_Select_Menu");
        Selected = bundle.LoadAsset<Sprite>("Select_Menu");

        // AssetBundle 해제
        bundle.Unload(false);
    }

    private void Update()
    {
        Function();
    }

    private void Function()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || (!isOnMenu && Input.GetKeyDown(KeyCode.Return)))
        {
            MenuOnOff();
        }
    }

    private void MenuOnOff()
    {
        if (!isOnMenu)
        {
            UI_Menu.SetActive(true);
            isOnMenu = true;
        }
        else if (isOnMenu)
        {
            UI_Menu.SetActive(false);
            isOnMenu = false;
        }
    }

    public static void ChangeScene()
    {
        SceneManager.LoadScene("BaseCamp");
    }
}