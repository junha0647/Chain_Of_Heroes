using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{

    private GridSystem gridSystem;
    private GridPosition gridPosition;
    private List<Character> characterList;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        characterList = new List<Character>();
    }

    public override string ToString()
    {
        string characterString = "";
        foreach (Character character in characterList)
        {
            characterString += character + "\n";
        }

        return gridPosition.ToString() + "\n" + characterString;
    }

    public void AddCharacter(Character character)
    {
        characterList.Add(character);
    }

    public void RemoveCharacter(Character character)
    {
        characterList.Remove(character);
    }

    public List<Character> GetCharacterList()
    {
        return characterList;
    }
}
