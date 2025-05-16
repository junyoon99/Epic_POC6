using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanSelectObject : MonoBehaviour
{
    public Action Select;
    public Action Deselect;
    public bool isSelected { get; private set; }
    GameObject selectBorder;

    public List<CanSelectObject> AttackingMe = new List<CanSelectObject>();
    public Action ChangedRoom;

    private void Awake()
    {
        // 선택표시 테두리 만들기
        selectBorder = new GameObject("SelectBorder");
        selectBorder.transform.SetParent(transform);
        selectBorder.transform.localPosition = Vector3.zero;
        SpriteRenderer borderSprite = selectBorder.AddComponent<SpriteRenderer>();
        borderSprite.sprite = GetComponent<SpriteRenderer>().sprite;
        borderSprite.color = Color.yellow;
        borderSprite.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
        selectBorder.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        selectBorder.SetActive(false);

        GameManager.Instance.AllUnits.Add(GetInstanceID(), gameObject);

        Select += Selected;
        Deselect += DeSelect;
    }

    public Room CheckCurrentRoom()
    {
        RaycastHit2D[] AllHits = Physics2D.RaycastAll(transform.position, Vector2.zero);
        foreach (RaycastHit2D hit in AllHits)
        {
            if (hit.collider.TryGetComponent<Room>(out Room currentRoom))
            {
                return currentRoom;
            }
        }

        Debug.LogError(gameObject.name + "의 Room현재 방을 찾을 수 없음!");
        return null;
    }

    private void OnDestroy()
    {
        Select -= Selected;
        Deselect -= DeSelect;
    }
    void Selected()
    {
        isSelected = true;
        selectBorder.SetActive(true);
    }

    void DeSelect()
    {
        isSelected = false;
        selectBorder.SetActive(false);
    }

    public void AddAttackingMeObject(CanSelectObject obj)
    {
        if (!AttackingMe.Contains(obj))
        {
            AttackingMe.Add(obj);
        }
    }

    public void RemoveAttackingMeObject(CanSelectObject obj)
    {
        if (AttackingMe.Contains(obj))
        {
            AttackingMe.Remove(obj);
        }
    }
}
