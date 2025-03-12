using UnityEngine;
using DG.Tweening;
using Zenject;

public class Notification : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float delayBeforeFade = 3f;

    private CanvasGroup canvasGroup;
    protected PopUpFactory _popUpFactory;

    [Inject]
    public void Construct(PopUpFactory popUpFactory)
    {
        _popUpFactory = popUpFactory;
    }
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup не найден! Добавьте CanvasGroup на объект.");
            return;
        }

        DOVirtual.DelayedCall(delayBeforeFade, () =>
        {
            canvasGroup.DOFade(0f, fadeDuration).OnComplete(() => _popUpFactory.ReturnNotificationToPool(this));
        });
    }
}
