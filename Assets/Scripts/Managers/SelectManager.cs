using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
public class SelectManager : MonoBehaviour
{
    public static SelectManager Instance 
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType(typeof(SelectManager)) as SelectManager;
                if (Instance == null)
                {
                    Debug.LogError("셀렉트매니저 없음!");
                }
            }
            return _instance;
        }
    }
    static SelectManager _instance;

    public List<CanSelectObject> SelectedObjects = new List<CanSelectObject>();
    GameObject DragBox;
    CanSelectObject targetObject;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if(_instance != this)
        {
            Destroy(this);
        }

        DragBox = Resources.Load<GameObject>("Prefabs/DragBox");
        DragBox = Instantiate(DragBox);
        DragBox.SetActive(false);
    }

    void Start()
    {
        InputManager.Instance.input.Ingame.M_LeftClick.started += TrySelect;
        InputManager.Instance.input.Ingame.M_LeftClick.started += Dragging;
        InputManager.Instance.input.Ingame.M_RightClick.started += Command;
    }

    // 단일 선택
    private void TrySelect(InputAction.CallbackContext ctx)
    {
        List<CanSelectObject> copy = new List<CanSelectObject>();
        foreach (CanSelectObject obj in SelectedObjects)
        {
            copy.Add(obj);
        }
        ClearSelectdObjects();

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hitObjects = Physics2D.RaycastAll(mousePosition, Vector2.zero);

        foreach (RaycastHit2D hit in hitObjects)
        {
            if (hit.collider.TryGetComponent<CanSelectObject>(out CanSelectObject canSelect) && !copy.Contains(hit.collider.GetComponent<CanSelectObject>()))
            {
                SelectedObjects.Add(canSelect);
                canSelect.Select.Invoke();
                return;
            }
        }
    }

    // 드래그
    private void Dragging(InputAction.CallbackContext ctx)
    {
        StartCoroutine(DrawDragBox());
    }

    IEnumerator DrawDragBox()
    {
        Vector2 startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 endPos = Vector2.zero;
        DragBox.SetActive(true);

        while (InputManager.Instance.input.Ingame.M_LeftClick.IsPressed())
        {
            endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 min = Vector2.Min(startPos, endPos);
            Vector2 max = Vector2.Max(startPos, endPos);

            DragBox.transform.position = (min + max) / 2;
            DragBox.transform.localScale = new Vector3(max.x - min.x, max.y - min.y);

            yield return null;
        }

        if (DragBox.transform.localScale.x > 0.1f && DragBox.transform.localScale.y > 0.1f)
        {
            Collider2D[] DragedObjects = Physics2D.OverlapAreaAll(startPos, endPos);
            ClearSelectdObjects();
            foreach (Collider2D col in DragedObjects)
            {
                if (col.GetComponent<CommandController>())
                {
                    SelectedObjects.Add(col.GetComponent<CanSelectObject>());
                    col.GetComponent<CanSelectObject>().Select.Invoke();
                }
            }
        }
        DragBox.SetActive(false);
    }

    void ClearSelectdObjects()
    {
        foreach (CanSelectObject obj in SelectedObjects)
        {
            obj.Deselect.Invoke();
        }
        SelectedObjects.Clear();
    }

    void Command(InputAction.CallbackContext ctx)
    {
        if (SelectedObjects.Count == 0) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hitObjects = Physics2D.RaycastAll(mousePosition, Vector2.zero);

        targetObject = null;
        Room ClickRoom = null;

        foreach (RaycastHit2D hit in hitObjects)
        {
            if (hit.collider.TryGetComponent<CanSelectObject>(out targetObject))
            {
                break;
            }
            else if (hit.collider.TryGetComponent<Core>(out Core core))
            {
                targetObject = core.GetComponent<CanSelectObject>();
                break;
            }

            if (hit.collider.GetComponent<Room>() != null)
            {
                ClickRoom = hit.collider.GetComponent<Room>();
            }
        }


        foreach (CanSelectObject obj in SelectedObjects)
        {
            if (obj.TryGetComponent<CommandController>(out CommandController commandObj))
            {
                commandObj.A_SetTarget.Invoke(targetObject, mousePosition, ClickRoom);
            }
        }
    }
}
