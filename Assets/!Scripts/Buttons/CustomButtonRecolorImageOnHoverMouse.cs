using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButtonRecolorImageOnHoverMouse : CustomButtonBase
{
    [SerializeField] private Color toColor;
    [SerializeField] Image image;
    private Color _originalColor;
    private Image _backgroundImage;

    private void Start()
    {
        _backgroundImage = image;
        _originalColor = _backgroundImage.color;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        _backgroundImage.DOColor(toColor, ANIMATION_DURATION)
            .SetEase(Ease.InOutSine);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        _backgroundImage.DOColor(_originalColor, ANIMATION_DURATION)
        .SetEase(Ease.InOutSine);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        _backgroundImage.DOColor(_originalColor, ANIMATION_DURATION)
        .SetEase(Ease.InOutSine);
    }
}
