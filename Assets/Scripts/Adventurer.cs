using UnityEngine;

public class Adventurer : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.AllAdventurers.Add(GetInstanceID(), gameObject);
    }
}
