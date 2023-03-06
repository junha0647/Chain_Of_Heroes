using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public enum pClass { Rook, Knight, Bishop, Queen, King }
[Serializable]
public class PlayerStatus
{
    public int ID;
    public string Name;
    public int Level;
    public int MaxLevel;

    // 파일 입출력 다시 좀 알아보기
    // 데이터베이스매니저? 에 대해서 알아보기
    // 딕셔너리 알아보기

    public float CurrentExp;
    public float MaxExp;
    public float AttackPower;
    public float ChainAttackPower;
    public float DefensePower;
    public float Hp;
    public float CriticalRate;
    public float CriticalDamage;
    public pClass Class;

    public void debugInfo()
    {
        Debug.Log(Name + ", " +
            Level + ", " +
            MaxLevel + ", " +
            CurrentExp + ", " +
            MaxExp + ", " +
            AttackPower + ", " +
            ChainAttackPower + ", " +
            DefensePower + ", " +
            Hp + ", " +
            CriticalRate + ", " +
            CriticalDamage + ", " +
            Class);
    }
}

public class JSONReader : MonoBehaviour
{ 
    [Serializable]
    public class Player
    {
        public PlayerStatus[] player;
    }
    public Player p = new Player();

    public static Dictionary<int, PlayerStatus> playerDic = new Dictionary<int, PlayerStatus>();

    private void Awake()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("JSON/filejson");

        p = JsonUtility.FromJson<Player>(textAsset.text);

        foreach (PlayerStatus ps in p.player)
        {
            playerDic.Add(ps.ID, ps);
        }
        //foreach(int pKey in  playerDic.Keys)
        //{
        //    playerDic[pKey].printInfo();
        //    Debug.Log("=============");
        //}
        foreach (KeyValuePair<int, PlayerStatus> ps in playerDic)
        {
            //Debug.Log(ps.Key);
            //ps.Value.printInfo();
            //Debug.Log("=============");
        }
    }
}