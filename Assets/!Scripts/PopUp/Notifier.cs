using UnityEngine;
using DG.Tweening;

public class Notifier : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float delayBeforeFade = 3f;

    private CanvasGroup canvasGroup;

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
            canvasGroup.DOFade(0f, fadeDuration).OnComplete(() => Destroy(gameObject));
        });
    }
}
