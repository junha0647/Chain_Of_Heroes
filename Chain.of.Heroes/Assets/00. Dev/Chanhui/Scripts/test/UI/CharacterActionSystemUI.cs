using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterActionSystemUI : MonoBehaviour
{

    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    private List<ActionButtonUI> actionButtonUIList;

    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        CharacterActionSystem.Instance.OnSelectedCharacterChanged += CharacterActionSystem_OnSelectedCharacterChanged;
        CharacterActionSystem.Instance.OnSelectedActionChanged += CharacterActionSystem_OnSelectedActionChanged;
        CharacterActionSystem.Instance.OnActionStarted += CharacterActionSystem_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Character.OnAnyActionPointsChanged += Character_OnAnyActionPointsChanged;

        UpdateActionPoints();
        CreateCharaterActionButtons();
        UpdateSelectedVisual();
    }


    private void CreateCharaterActionButtons()
    {
        foreach(Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIList.Clear();

        Character selectedCharacter = CharacterActionSystem.Instance.GetSelecterdCharacter();

        foreach(BaseAction baseAction in selectedCharacter.GetBaseActionArray())
        {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);

            actionButtonUIList.Add(actionButtonUI);
        }
    }

    private void CharacterActionSystem_OnSelectedCharacterChanged(object sender, EventArgs e)
    {
        CreateCharaterActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void CharacterActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void CharacterActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UpdateSelectedVisual()
    {
        foreach(ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints()
    {
        Character selectedCharacter = CharacterActionSystem.Instance.GetSelecterdCharacter();

        actionPointsText.text = "Action Points: " + selectedCharacter.GetActionPoints();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void Character_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }
}
