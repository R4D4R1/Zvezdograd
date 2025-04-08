using UnityEngine;
using Cysharp.Threading.Tasks;

public class MonoInit : MonoBehaviour
{
    public virtual UniTask Init()
    {
        //Debug.Log($"{name} - Initialized");
        return UniTask.CompletedTask;
    }
}