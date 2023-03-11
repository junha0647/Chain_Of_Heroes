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

    #region �̵�
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

    #region �޴� ����
    private void MenuFunction()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (currentSelected.name)
            {
                case "Image_ChapterStart":
                    Debug.Log("é�� ����");

                    // �ý��� �ʿ�

                    break;
                case "Image_Inventory":
                    Debug.Log("����ǰ");

                    // �ý��� �ʿ�

                    break;
                case "Image_Party":
                    Debug.Log("����");

                    // �ý��� �ʿ�

                    break;
                case "Image_Save":
                    Debug.Log("����");

                    // �ý��� �ʿ�
                    // �׷��� �⺻ Ʋ ���� ����.

                    break;
                case "Image_GoToBaseCamp":
                    Debug.Log("���̽� ķ����");

                    // ���̵� �� ��� �ʿ��Ѱ�?
                    UIManager.ChangeScene();

                    break;
            }
        }
    }
    #endregion
}
