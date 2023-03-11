using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectMenuArrowController : MonoBehaviour
{
    private const float MoveDistance = 101f;
    private const KeyCode UpKey = KeyCode.UpArrow;
    private const KeyCode DownKey = KeyCode.DownArrow;
    private RectTransform rt;

    [SerializeField] private GameObject currentSelected;
    [SerializeField]private Image currentImage;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        currentImage = currentSelected.GetComponent<Image>();

        currentSelected = EventSystem.current.firstSelectedGameObject;
    }

    private void OnEnable()
    {
        rt.anchoredPosition = new Vector2(-160f, -260f);

        currentSelected = EventSystem.current.firstSelectedGameObject;
    }

    private void Update()
    {
        Movement();

        MenuFunction();
    }

    #region 이동
    private void Movement()
    {
        if (Input.GetKeyDown(UpKey))
        {
            rt.anchoredPosition += Vector2.up * MoveDistance;

            currentSelected = currentSelected.GetComponent<Selectable>().FindSelectableOnUp().gameObject;
            currentSelected.GetComponent<Selectable>().Select();

            currentImage = currentSelected.GetComponent<Image>();
            currentImage.sprite = UIManager.Selected;
        }
        else if (Input.GetKeyDown(DownKey))
        {
            rt.anchoredPosition += -Vector2.up * MoveDistance;

            currentSelected = currentSelected.GetComponent<Selectable>().FindSelectableOnDown().gameObject;
            currentSelected.GetComponent<Selectable>().Select();
            
            currentImage = currentSelected.GetComponent<Image>();
            currentImage.sprite = UIManager.Selected;
        }
    }
    #endregion

    #region 메뉴 선택
    private void MenuFunction()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (currentSelected.name)
            {
                case "Image_ChapterStart":
                    Debug.Log("챕터 시작");

                    // 시스템 필요

                    break;
                case "Image_Inventory":
                    Debug.Log("소지품");

                    // 시스템 필요

                    break;
                case "Image_Party":
                    Debug.Log("동료");

                    // 시스템 필요

                    break;
                case "Image_Save":
                    Debug.Log("저장");

                    // 시스템 필요
                    // 그러나 기본 틀 구현 가능.

                    break;
                case "Image_GoToBaseCamp":
                    Debug.Log("베이스 캠프로");

                    // 페이드 인 기능 필요한가?
                    UIManager.ChangeScene();

                    break;
            }
        }
    }
    #endregion
}
