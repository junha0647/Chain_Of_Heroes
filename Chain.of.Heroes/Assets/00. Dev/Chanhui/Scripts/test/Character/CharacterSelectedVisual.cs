using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectedVisual : MonoBehaviour
{
    [SerializeField] private Character character;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        CharacterActionSystem.Instance.OnSelectedCharacterChanged += CharacterActionSystem_OnSelectedCharacterChanged;

        UpdateVisual();
    }

    private void CharacterActionSystem_OnSelectedCharacterChanged(object sender, EventArgs empty)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (CharacterActionSystem.Instance.GetSelecterdCharacter() == character)
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }
}
