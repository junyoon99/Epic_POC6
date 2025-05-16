using UnityEngine;

public class Monster : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.AllMonsters.Add(GetInstanceID(), gameObject);
    }
}
