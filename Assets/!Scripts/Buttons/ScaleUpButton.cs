using DG.Tweening;
using UnityEngine.EventSystems;

public class ScaleUpButton : CustomButtonBase
{
    private const float ORIGINAL_SCALE = 1.0f;
    private const float ON_HOVER_SCALE_MULTIPLIER = 1.05f;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        transform.DOScale(ORIGINAL_SCALE * ON_HOVER_SCALE_MULTIPLIER, ANIMATION_DURATION)
            .SetEase(Ease.InOutSine);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        transform.DOScale(ORIGINAL_SCALE, ANIMATION_DURATION)
            .SetEase(Ease.InOutSine);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        transform.DOScale(ORIGINAL_SCALE, ANIMATION_DURATION)
        .SetEase(Ease.InOutSine);
    }
}
