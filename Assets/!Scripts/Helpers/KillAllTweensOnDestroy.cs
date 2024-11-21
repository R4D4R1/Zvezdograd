using DG.Tweening;
using UnityEngine;

public class KillAllTweensOnDestroy : MonoBehaviour
{
    void OnDestroy()
    {
        DOTween.KillAll(complete: false);
    }
}