using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ReturnToPoolPopUp : InfoPopUp
{
    private const float RETURN_TO_POOL_TIME_SECONDS = 1f;

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
        await UniTask.Delay((int)(RETURN_TO_POOL_TIME_SECONDS * 1000));
        _popUpFactory.ReturnInfoPopUpToPool(this);
    }
}
