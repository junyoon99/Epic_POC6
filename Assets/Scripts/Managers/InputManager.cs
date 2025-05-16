using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance 
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType(typeof(InputManager)) as InputManager;
                if (Instance == null)
                {
                    Debug.LogError("��ǲ�Ŵ�������!");
                }
            }
            return _instance;
        }
    }
    static InputManager _instance;

    public PlayerInput input;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if(Instance != this)
        {
            Destroy(this);
        }

        input = new PlayerInput();
        input.Enable();
    }
}
