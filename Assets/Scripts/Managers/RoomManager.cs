using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance
    {
        get
        {
            if (_instance == null)
            {
               _instance = FindAnyObjectByType(typeof(RoomManager)) as RoomManager;
                if (Instance == null)
                {
                    Debug.LogError("룸매니저 없음!");
                }
            }
            return _instance;
        }
    }
    static RoomManager _instance;


    public List<Room> AllRooms = new List<Room>();
    public List<Door> AllDoors = new List<Door>();

    private void Awake()
    {
        if (_instance == null) 
        {
            _instance = this;
        }
        else if(_instance != this)
        {
            Destroy(this);
        }
    }
}
