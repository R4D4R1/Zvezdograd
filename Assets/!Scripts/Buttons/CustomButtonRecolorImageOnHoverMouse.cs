using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButtonRecolorImageOnHoverMouse : CustomButtonBase
{
    private Image _backgroundImage;
    [SerializeField] private float _duration;
    [SerializeField] private Color _toColor;
    [SerializeField] Image _image;
    private Color _originalColor;

    private void Start()
    {
        _backgroundImage = _image;
        _originalColor = _backgroundImage.color;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        _backgroundImage.DOColor(_toColor, _duration)
            .SetEase(Ease.InOutSine);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        _backgroundImage.DOColor(_originalColor, _duration)
        .SetEase(Ease.InOutSine);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        _backgroundImage.DOColor(_originalColor, _duration)
        .SetEase(Ease.InOutSine);
    }
}
