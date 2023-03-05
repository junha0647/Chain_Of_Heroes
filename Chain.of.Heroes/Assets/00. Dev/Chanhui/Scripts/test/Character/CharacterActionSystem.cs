using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActionSystem : MonoBehaviour
{
    public static CharacterActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;

    [SerializeField] private Character selectedCharacter;
    [SerializeField] private LayerMask CharacterLayerMask;

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

    private void Update()
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            if (TryHandleCharacterSelection()) { return; }

            selectedCharacter.Move(MouseWorld.GetPosition());
        }
    }

    private bool TryHandleCharacterSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, CharacterLayerMask))
        {
            if(raycastHit.transform.TryGetComponent<Character>(out Character character))
            {
                SetSelectedCharacter(character);
                return true;
            }
        }

        return false;
    }

    private void SetSelectedCharacter(Character character)
    {
        selectedCharacter = character;

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public Character GetSelecterdCharacter()
    {
        return selectedCharacter;
    }
}
