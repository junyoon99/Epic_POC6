using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    static CameraManager _instance;
    bool isClicked;
    CinemachineCamera activeCam;
    public static CameraManager Instance 
    {
        get 
        {
            if (!_instance) 
            {
                _instance = FindAnyObjectByType(typeof(CameraManager)) as CameraManager;
                if (!_instance) 
                {
                    print("NO CameraManager Instance");
                }
            }
            return _instance;
        }
    }
    void Awake()
    {
        if (!_instance) 
        {
            _instance = this;
        }
        else if(_instance != this)
        {
            Destroy( this );
        }
    }

    private void Start()
    {
        InputManager.Instance.input.Ingame.M_Wheel.started += Zoom;
        InputManager.Instance.input.Ingame.M_WheelClick.started += ClickStart;
        InputManager.Instance.input.Ingame.M_WheelClick.canceled += ClickEnd;

        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        activeCam = brain.ActiveVirtualCamera as CinemachineCamera;
        if (activeCam == null)
        {
            Debug.Log("¾¾¹ß ³Ê Á¿µÆ¾î!");
        }
    }

    private void Update()
    {
        DragCamera();
    }

    void Zoom(InputAction.CallbackContext ctx)
    {
        activeCam.Lens.OrthographicSize += ctx.ReadValue<float>();
    }
    void DragCamera()
    {
        Vector2 inputValue = InputManager.Instance.input.Ingame.M_MouseMove.ReadValue<Vector2>();
        if (inputValue != Vector2.zero && isClicked)
        {
            activeCam.transform.position -= new Vector3(inputValue.x / 40, inputValue.y / 40, 0);
        }
    }

    void ClickStart(InputAction.CallbackContext ctx) 
    {
        isClicked = true;
    }

    void ClickEnd(InputAction.CallbackContext ctx) 
    {
        isClicked= false;
    }
}
