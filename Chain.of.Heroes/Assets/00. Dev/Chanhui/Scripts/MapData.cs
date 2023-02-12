using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    GoblinArcher = 0,
    GoblinSpear = 1,
    GoblinSword = 2
}

[CreateAssetMenu(fileName = "Monster Data", menuName = "Scriptable Object/Monster Data")]
public class MapData : ScriptableObject
{
    [Header("Map_Information")]
    [SerializeField] private string Map_Id;
    [SerializeField] private string Map_Name;
    [SerializeField] private bool Map_Clear_Confirm;

    [Header("Monster_Information")]
    [SerializeField] private GameObject[] monster_pf;
    public GameObject[] Monster_pf { get { return monster_pf; } }

    [SerializeField] private MonsterType[] type;
    public MonsterType[] Type { get { return type; } }

    [SerializeField] private int monster_num;
    public int MONSTER_NUM { get { return monster_num; } }

    [SerializeField] private Vector2[] currentXY;
    public Vector2[] CurrentXY { get { return currentXY; } }

}
