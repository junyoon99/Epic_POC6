using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    // Tuple�� ���� ����� ��� �ε����� ���� �� ǥ���ϱ� ����
    public List<Tuple<Door, int>> FindPath(CanSelectObject moveObject, Room targetRoom)
    {
        if (!moveObject || !targetRoom) return null;
        // Ž�������� ť
        PriorityQueue<Door> openSet = new PriorityQueue<Door>();
        // ������ �� ����
        Dictionary<Door, Tuple<Door, int>> cameFrom = new Dictionary<Door, Tuple<Door, int>>();
        // ������ �Ÿ�, ���� �Ÿ� ���� �� ��
        Dictionary<Door, float> gScore = new Dictionary<Door, float>();
        // ������ �Ÿ� + ���� �Ÿ� ���� ��
        Dictionary<Door, float> fScore = new Dictionary<Door, float>();

        List<Door> AllDoors = RoomManager.Instance.AllDoors;
        Room currentRoom = moveObject.CheckCurrentRoom();
        if (currentRoom == targetRoom) return new List<Tuple<Door, int>>();

        // ��� ���� �ִ����� �ʱ�ȭ
        foreach (Door door in AllDoors)
        {
            gScore[door] = float.MaxValue;
            fScore[door] = float.MaxValue;
        }

        // ù ��ȿ� �ִ� ��� ������ �������� ���� �־���
        foreach (Door startRoomDoors in currentRoom.Doors.Values)
        {
            // ���۹��� ������ ������Ʈ�� ���� �Ÿ�, �����Ÿ��� �������� ���� ���� ��ŭ���� ����
            gScore[startRoomDoors] = Vector2.Distance(moveObject.transform.position, startRoomDoors.transform.position);
            fScore[startRoomDoors] = Vector2.Distance(startRoomDoors.transform.position, targetRoom.transform.position);

            openSet.Enqueue(startRoomDoors, fScore[startRoomDoors]);
        }

        while (openSet.Count > 0)
        {
            // ���� f���� ���� ���๮ ������
            Door currentDoor = openSet.Dequeue();

            // ���� �� ���� �������濡 �ִ� ���̸� �� ������ ���� ��� ����
            if (currentDoor.currentRoom == targetRoom)
            {
                return ReconstructPath(cameFrom, currentDoor);
            }
            List<Door> currentConnectedDoor = currentDoor.connectedDoor;
            int index = 0;

            // �װ� �ƴ϶�� ����� ��� ������ Ȯ���ϰ� �߰�
            foreach (Door doors in currentDoor.connectedDoor)
            {
                // ������ �𸣰ڴµ� ����Ʈ�� ����־ foreach���� ���Ƽ� null�� ����
                if (doors == null) continue;

                foreach (Door door in doors.currentRoom.Doors.Values)
                {
                    // ���������� ���� ����� ���ݱ��� ������ �Ÿ� + �̾��������� ���������� �̵� �� �� �߻��ϴ� �Ÿ�
                    float tentativeG = gScore[currentDoor] + Vector3.Distance(doors.transform.position, door.transform.position);

                    // ���� ����� ����� ���� ����Ǿ��ִ� �� ������� �Ÿ����� �۴ٸ�
                    if (tentativeG < gScore[door])
                    {
                        // �� ������ ���� ����� ���� �ִ� ���� ���ؼ� ���� ���� ��
                        cameFrom[door] = Tuple.Create(currentDoor, index);

                        gScore[door] = tentativeG;
                        fScore[door] = tentativeG + Vector2.Distance(door.transform.position, targetRoom.transform.position);

                        // Ž�� ����� ����߿� �������� ����Ǿ������� ���� ������Ʈ
                        if (openSet.Contains(door))
                        {
                            openSet.UpdatePriority(door, fScore[door]);
                        }
                        // ���࿡ ���� ���̸� �߰�
                        else
                        {
                            openSet.Enqueue(door, fScore[door]);
                        }
                    }
                }
                index++;
            }

        }
        Debug.Log("�� ã�� �� ����!");
        return null;
    }

    // ���� �Լ�
    private List<Tuple<Door, int>> ReconstructPath(Dictionary<Door, Tuple<Door, int>> cameFrom, Door endDoor)
    {
        List<Tuple<Door, int>> path = new List<Tuple<Door, int>>();
        Door current = endDoor;

        while (cameFrom.ContainsKey(current))
        {
            path.Add(cameFrom[current]);
            current = cameFrom[current].Item1;
        }

        return path;
    }
}
