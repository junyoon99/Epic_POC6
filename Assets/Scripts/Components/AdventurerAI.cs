using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdventurerAI : MonoBehaviour
{
    // �־�� �ϴ� ������Ʈ��
    PathFinder pathFinder;
    Attack attack;
    Move move;
    CanSelectObject canSelectObject;
    Collider2D col;

    //Ÿ��
    public CanSelectObject targetObject;

    // �� ����
    public List<Tuple<Door, int>> path = new List<Tuple<Door, int>>();

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        pathFinder = GetComponent<PathFinder>();
        attack = GetComponent<Attack>();
        move = GetComponent<Move>();
        canSelectObject = GetComponent<CanSelectObject>();

        canSelectObject.ChangedRoom += PathUpdate;
        move.Arrived += isArrived;
    }

    private void Start()
    {
        CheckMonsterInRoom(canSelectObject.CheckCurrentRoom());
    }

    private void Update()
    {
        if (!targetObject)
        {
            findRandomEnemy();
        }
    }

    void findRandomEnemy() 
    {
        List<GameObject> allMonstersList = GameManager.Instance.AllMonsters.Values.ToList();
        SetTarget(allMonstersList[UnityEngine.Random.Range(0, allMonstersList.Count)].GetComponent<CanSelectObject>());
    }

    private void PathUpdate() 
    {
        print("��� ������Ʈ");
        SetTarget(targetObject);
    }

    public void SetTarget(CanSelectObject targetObject) 
    {
        if (this.targetObject && this.targetObject != targetObject)
        {
            targetObject.RemoveAttackingMeObject(canSelectObject);
        }
        if (targetObject) 
        {
            targetObject.AddAttackingMeObject(canSelectObject);
        }

        this.targetObject = targetObject;
        if (targetObject) 
        {
            path = pathFinder.FindPath(canSelectObject, targetObject.CheckCurrentRoom());
            attack.SetAttackTarget(targetObject);
            if (path.Count > 0)
            {
                move.SetTarget(null, path[path.Count - 1].Item1.transform.position);
            }
            else
            {
                move.SetTarget(targetObject, null);
            }
        }
    }

    void isArrived() 
    {
        if (path.Count > 0) 
        {
            print("next room!");
            TryEnterDoor();
        }
    }

    void TryEnterDoor() 
    {
        Collider2D[] inObjs = Physics2D.OverlapAreaAll(col.bounds.min, col.bounds.max);
        Door connectedDoor = null ;

        foreach (Collider2D inObj in inObjs)
        {
            if (inObj.TryGetComponent<Door>(out Door door))
            {
                connectedDoor = door.DoorEnter(transform, path[path.Count - 1].Item2);
                canSelectObject.ChangedRoom?.Invoke();
                break;
            }
        }
        // �� �����Ϸ� ���� �ֵ����� �� �ٲ�ٰ� �˷���
        foreach (CanSelectObject attacker in canSelectObject.AttackingMe)
        {
            attacker.ChangedRoom?.Invoke();
        }
        // �� �濡 ���Ͱ� �ִ��� Ȯ����
        CheckMonsterInRoom(connectedDoor.currentRoom);
    }

    void CheckMonsterInRoom(Room currentRoom) 
    {
        if (currentRoom != null)
        {
            List<CanSelectObject> monsters = currentRoom.CheckObjectInRoom(Room.findType.Monster);
            if (monsters.Count != 0)
            {
                CanSelectObject closestMonster = null;
                float closestDistance = float.MaxValue;
                foreach (CanSelectObject monster in monsters)
                {
                    if (Vector2.Distance(transform.position, monster.transform.position) < closestDistance)
                    {
                        closestDistance = Vector2.Distance(transform.position, monster.transform.position);
                        closestMonster = monster;
                    }
                }
                SetTarget(closestMonster);
            }
        }
        else
        {
            Debug.LogError("connectedDoor.currentRoom is null!");
        }
    }

    private void OnDestroy()
    {
        canSelectObject.ChangedRoom -= PathUpdate;
        move.Arrived -= isArrived;
    }
}
