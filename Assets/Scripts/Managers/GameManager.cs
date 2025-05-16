using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance 
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType(typeof(GameManager)) as GameManager;
                if (_instance == null)
                {
                    Debug.LogError("게임 매니저 없음!");
                }
            }
            return _instance;
        }
    }

    public Dictionary<int, GameObject> AllUnits = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> AllMonsters = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> AllAdventurers = new Dictionary<int, GameObject>();

    public List<GameObject> TestAllUnits = new List<GameObject>();
    public List<GameObject> TestAllMonsters = new List<GameObject>();
    public List<GameObject> TestAllAdventurers = new List<GameObject>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        TestAllUnits.Clear();
        TestAllMonsters.Clear();
        TestAllAdventurers.Clear();
        foreach (var item in AllUnits)
        {
            TestAllUnits.Add(item.Value);
        }
        foreach (var item in AllMonsters)
        {
            TestAllMonsters.Add(item.Value);
        }
        foreach (var item in AllAdventurers)
        {
            TestAllAdventurers.Add(item.Value);
        }
    }

    public void SummonMonster(int level) 
    {
        print("소환 레벨: " + level);
    }
}
