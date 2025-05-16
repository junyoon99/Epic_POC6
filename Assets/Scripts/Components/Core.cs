using UnityEngine;

public abstract class Core : MonoBehaviour
{
    public string CoreName;
    public int CoreLevel;
    public float CoolTime;
    public int CanUseCountInDay;
    public abstract void Interact(CanSelectObject interactor);
}
