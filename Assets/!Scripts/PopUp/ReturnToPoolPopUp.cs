using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class ReturnToPoolPopUp : InfoPopUp
{
    [FormerlySerializedAs("_returnToPoolTimeInSeconds")] [Range(1, 2), SerializeField] private float returnToPoolTimeInSeconds;

    public override void HidePopUp()
    {
        if (IsActive)
        {
            IsActive = false;

            transform.DOScale(Vector3.zero, SCALE_DURATION).OnComplete(() =>
            {
                DelayAndReturnToPool().Forget();
            });
        }
    }

    private async UniTaskVoid DelayAndReturnToPool()
    {
        await UniTask.Delay((int)(returnToPoolTimeInSeconds * 1000));
        PopUpFactory.ReturnInfoPopUpToPool(this);
    }
}
