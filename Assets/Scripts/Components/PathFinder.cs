using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    // Tuple은 문에 연결된 어디 인덱스로 나올 지 표시하기 위해
    public List<Tuple<Door, int>> FindPath(CanSelectObject moveObject, Room targetRoom)
    {
        if (!moveObject || !targetRoom) return null;
        // 탐색예약할 큐
        PriorityQueue<Door> openSet = new PriorityQueue<Door>();
        // 지나온 곳 저장
        Dictionary<Door, Tuple<Door, int>> cameFrom = new Dictionary<Door, Tuple<Door, int>>();
        // 지나온 거리, 남은 거리 저장 할 맵
        Dictionary<Door, float> gScore = new Dictionary<Door, float>();
        // 지나온 거리 + 남은 거리 저장 맵
        Dictionary<Door, float> fScore = new Dictionary<Door, float>();

        List<Door> AllDoors = RoomManager.Instance.AllDoors;
        Room currentRoom = moveObject.CheckCurrentRoom();
        if (currentRoom == targetRoom) return new List<Tuple<Door, int>>();

        // 모든 문에 최댓값으로 초기화
        foreach (Door door in AllDoors)
        {
            gScore[door] = float.MaxValue;
            fScore[door] = float.MaxValue;
        }

        // 첫 방안에 있는 모든 문들은 수동으로 예약 넣어줌
        foreach (Door startRoomDoors in currentRoom.Doors.Values)
        {
            // 시작방은 움직일 오브젝트와 남은 거리, 남은거리는 목적지와 차이 길이 만큼으로 시작
            gScore[startRoomDoors] = Vector2.Distance(moveObject.transform.position, startRoomDoors.transform.position);
            fScore[startRoomDoors] = Vector2.Distance(startRoomDoors.transform.position, targetRoom.transform.position);

            openSet.Enqueue(startRoomDoors, fScore[startRoomDoors]);
        }

        while (openSet.Count > 0)
        {
            // 가장 f값이 낮은 예약문 꺼내기
            Door currentDoor = openSet.Dequeue();

            // 지금 그 문이 목적지방에 있는 문이면 이 문까지 오는 방법 리턴
            if (currentDoor.currentRoom == targetRoom)
            {
                return ReconstructPath(cameFrom, currentDoor);
            }
            List<Door> currentConnectedDoor = currentDoor.connectedDoor;
            int index = 0;

            // 그게 아니라면 방안의 모든 문들을 확인하고 추가
            foreach (Door doors in currentDoor.connectedDoor)
            {
                // 왜인진 모르겠는데 리스트가 비어있어도 foreach문이 돌아서 null이 나옴
                if (doors == null) continue;

                foreach (Door door in doors.currentRoom.Doors.Values)
                {
                    // 다음문까지 가는 비용은 지금까지 지나온 거리 + 이어진문에서 다음방으로 이동 할 때 발생하는 거리
                    float tentativeG = gScore[currentDoor] + Vector3.Distance(doors.transform.position, door.transform.position);

                    // 만약 계산한 비용이 원래 저장되어있던 그 방까지의 거리보다 작다면
                    if (tentativeG < gScore[door])
                    {
                        // 그 문까지 가는 방법은 지금 있는 문을 통해서 가면 가장 쌈
                        cameFrom[door] = Tuple.Create(currentDoor, index);

                        gScore[door] = tentativeG;
                        fScore[door] = tentativeG + Vector2.Distance(door.transform.position, targetRoom.transform.position);

                        // 탐색 예약된 방들중에 다음방이 예약되어있으면 값만 업데이트
                        if (openSet.Contains(door))
                        {
                            openSet.UpdatePriority(door, fScore[door]);
                        }
                        // 예약에 없는 방이면 추가
                        else
                        {
                            openSet.Enqueue(door, fScore[door]);
                        }
                    }
                }
                index++;
            }

        }
        Debug.Log("길 찾을 수 없음!");
        return null;
    }

    // 배출 함수
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
