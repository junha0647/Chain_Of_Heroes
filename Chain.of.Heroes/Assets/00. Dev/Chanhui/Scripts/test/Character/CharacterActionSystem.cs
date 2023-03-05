using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterActionSystem : MonoBehaviour
{
    public static CharacterActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedCharacterChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    [SerializeField] private Character selectedCharacter;
    [SerializeField] private LayerMask CharacterLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There's more than one CharacterActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SetSelectedCharacter(selectedCharacter);
    }

    private void Update()
    {
        if(isBusy)
        {
            return;
        }

        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (TryHandleCharacterSelection())
        { 
            return; 
        }

        HandleSelectedAction();
    }

    // 움직임 결정
    private void HandleSelectedAction()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                return;
            }

            if (!selectedCharacter.TrySpendActionPointsToTakeAction(selectedAction))
            {
                return;
            }

            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);

            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SetBusy()
    {
        isBusy = true;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private bool TryHandleCharacterSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, CharacterLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Character>(out Character character))
                {
                    if(character == selectedCharacter)
                    {
                        // character is already selected
                        return false;
                    }

                    SetSelectedCharacter(character);
                    return true;
                }
            }
        }

        return false;
    }

    private void SetSelectedCharacter(Character character)
    {
        selectedCharacter = character;

        SetSelectedAction(character.GetMoveAction());

        OnSelectedCharacterChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Character GetSelecterdCharacter()
    {
        return selectedCharacter;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }
}
