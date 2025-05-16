using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CommandController : MonoBehaviour
{

    public Action<CanSelectObject, Vector2?, Room> A_SetTarget;
    // 있어야하는 컴포넌트들
    PathFinder pathFinder;
    CanSelectObject canSelectObject;
    Move move;
    Attack attack;
    Collider2D col;
    Interact interact;

    // 타겟
    public CanSelectObject targetObject;
    public Vector2? targetPosition;
    public Room targetRoom;

    // 길찾기, 인식 범위
    public List<Tuple<Door, int>> path = new List<Tuple<Door, int>>();

    // 테스트(인스펙터 확인용)
    public Vector2 TesttargetPosition;
    public List<Door> TestPath = new List<Door>();

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        pathFinder = GetComponent<PathFinder>();
        attack = GetComponent<Attack>();
        move = GetComponent<Move>();
        canSelectObject = GetComponent<CanSelectObject>();
        interact = GetComponent<Interact>();

        move.Arrived += isArrived;
        A_SetTarget += SetTarget;
        canSelectObject.ChangedRoom += PathUpdate;
    }

    private void Update()
    {
        TesttargetPosition = targetPosition != null ? (Vector2)targetPosition : Vector2.zero;
        TestPath.Clear();
        foreach (var item in path)
        {
            TestPath.Add(item.Item1);
        }
    }
    private void PathUpdate()
    {
        print(targetObject ? targetObject.name : "null" + " " + targetPosition + " " + (targetRoom ? targetRoom.name : "null"));
        SetTarget(targetObject, targetPosition, targetRoom);
    }

    void SetTarget(CanSelectObject targetObject, Vector2? targetPosition, Room targetRoom)
    {
        // 타겟이 있었는데 새로 settarget할 때 기존 타겟과 다르면 기존 타겟의 공격자 리스트에서 뺌
        if (targetObject != this.targetObject && this.targetObject)
        {
            this.targetObject.RemoveAttackingMeObject(canSelectObject);
        }
        // 상대가 있으면 그 상대한테 내가 공격하러 간다고 알려줌
        if (targetObject && targetObject.GetComponent<Adventurer>())
        {
            targetObject.AddAttackingMeObject(canSelectObject);
        }

        this.targetObject = targetObject;
        this.targetPosition = targetPosition;
        this.targetRoom = targetRoom;
        if (targetObject && targetObject.GetComponent<Adventurer>())
        {
            FindPath(targetObject);
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
        else if (targetObject && targetObject.TryGetComponent<Core>(out Core core)) 
        {
            FindPath(targetObject);
            attack.DisableAttack();
            interact.SetTarget(core);

            if (path.Count > 0)
            {
                move.SetTarget(null, path[path.Count - 1].Item1.transform.position);
            }
            else
            {
                move.SetTarget(targetObject, null);
            }
        }
        else if (targetRoom != null)
        {
            FindPath(targetRoom);
            attack.DisableAttack();
            interact.ComponentDisable();
            if (path.Count > 0)
            {
                move.SetTarget(null, path[path.Count - 1].Item1.transform.position);
            }
            else if (targetPosition != null)
            {
                move.SetTarget(null, targetPosition);
            }
        }
    }

    void FindPath(CanSelectObject target) 
    {
        path = pathFinder.FindPath(canSelectObject, target.CheckCurrentRoom());
    }
    void FindPath(Room target) 
    {
        path = pathFinder.FindPath(canSelectObject, target);
    }

    void isArrived() 
    {
        if (path.Count > 0) 
        {
            TryEnterDoor();
        }
        else if(targetPosition != null)
        {
            targetPosition = null;
            move.ComponentDisable();
        }
    }

    void TryEnterDoor()
    {
        Collider2D[] inObjs = Physics2D.OverlapAreaAll(col.bounds.min, col.bounds.max);
        foreach (Collider2D inObj in inObjs) 
        {
            if (inObj.TryGetComponent<Door>(out Door door))
            {
                door.DoorEnter(transform, path[path.Count - 1].Item2);
                canSelectObject.ChangedRoom?.Invoke();
                break;
            }
        }

        // 날 공격하러 오는 애들한테 방 바꿨다고 알려줌
        foreach (CanSelectObject attacker in canSelectObject.AttackingMe) 
        {
            print(attacker.name + "에게 방 바꿨다고 알려줌");
            attacker.ChangedRoom?.Invoke();
        }
    }

    private void OnDestroy()
    {
        A_SetTarget -= SetTarget;
        canSelectObject.ChangedRoom -= PathUpdate;
        move.Arrived -= isArrived;
    }
}
