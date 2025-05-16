using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public List<Door> connectedDoor = new List<Door>();
    public Room currentRoom;

    private void Awake()
    {
        RoomManager.Instance.AllDoors.Add(this);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.TryGetComponent<Room>(out Room room))
            {
                currentRoom = room;
                currentRoom.Doors.Add(gameObject.name, this);
                break;
            }
        }
    }

    private void Update()
    {
        if (connectedDoor.Count == 0)
        {
            gameObject.SetActive(false);
        }
    }

    public Door DoorEnter(Transform user, int index)
    {
        user.position = connectedDoor[index].transform.position;
        return connectedDoor[index];
    }
}
