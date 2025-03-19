using UnityEngine;
using DG.Tweening;
using Zenject;
using Cysharp.Threading.Tasks;

public class Notification : MonoBehaviour
{
    [Range(0.1f, 3.0f), SerializeField] private float fadeDuration = 1f;
    [Range(0.1f, 5.0f), SerializeField] private float delayBeforeFade = 3f;

    protected PopUpFactory _popUpFactory;
    private CanvasGroup _canvasGroup;

    [Inject]
    public void Construct(PopUpFactory popUpFactory)
    {
        _popUpFactory = popUpFactory;
    }

    private async void OnEnable()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_canvasGroup == null)
        {
            Debug.LogError("NO CANVAS GROUP!");
            return;
        }

        //Delay before destroy
        await UniTask.Delay((int)(delayBeforeFade * 1000));

        _canvasGroup.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                _popUpFactory.ReturnNotificationToPool(this);
                _canvasGroup.alpha = 1f;
            });
    }
}
