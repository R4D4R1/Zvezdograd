using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButtonScale : CustomButtonBase
{
    [SerializeField] private float _originalScale = 1.0f;
    [SerializeField] private float _toScale;
    [SerializeField] private float _duration;


    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        transform.DOScale(_toScale, _duration)
            .SetEase(Ease.InOutSine);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        transform.DOScale(_originalScale, _duration)
            .SetEase(Ease.InOutSine);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        transform.DOScale(_originalScale, _duration)
        .SetEase(Ease.InOutSine);
    }
}
