using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Dictionary<string, Door> Doors = new Dictionary<string, Door>();
    Core core;
    public enum findType { Monster, Adventurer };

    private void Awake()
    {
        RoomManager.Instance.AllRooms.Add(this);
    }

    public List<CanSelectObject> CheckObjectInRoom(findType findType)
    {
        List<CanSelectObject> result = new List<CanSelectObject>();
        Collider2D[] InRoomObjects = new Collider2D[50];
        Physics2D.OverlapCollider(GetComponent<Collider2D>(), new ContactFilter2D(), InRoomObjects);

        foreach (Collider2D col in InRoomObjects) 
        {
            if (col == null) continue;
            switch (findType) 
            {
                case findType.Monster:
                    if (col.TryGetComponent<Monster>(out Monster monster) || col.TryGetComponent<Core>(out Core Cores))
                    {
                        result.Add(col.GetComponent<CanSelectObject>());
                    }
                    break;
                case findType.Adventurer:
                    if (col.TryGetComponent<Adventurer>(out Adventurer adventurer))
                    {
                        result.Add(col.GetComponent<CanSelectObject>());
                    }
                    break;
            }
        }

        return result;
    }
}
