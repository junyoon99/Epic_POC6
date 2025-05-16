using UnityEngine;

public class SummonCore : Core
{
    public override void Interact(CanSelectObject interactor)
    {
        if (CanUseCountInDay <= 0)
        {
            return;
        }
        GameManager.Instance.SummonMonster(CoreLevel);
        CanUseCountInDay--;
    }

    private void Awake()
    {
        GameManager.Instance.AllMonsters.Add(GetInstanceID(), gameObject);
    }
}
