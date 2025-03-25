using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ReturnToPoolPopUp : InfoPopUp
{
    [Range(1, 2), SerializeField] private float _returnToPoolTimeInSeconds;

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
        await UniTask.Delay((int)(_returnToPoolTimeInSeconds * 1000));
        PopUpFactory.ReturnInfoPopUpToPool(this);
    }
}
