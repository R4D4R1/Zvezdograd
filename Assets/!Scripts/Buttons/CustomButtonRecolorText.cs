using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButtonRecolorText : CustomButtonBase
{
    private TMP_Text _buttonText;
    private Color _originalColor;
    [SerializeField] private Color _toColor;
    [SerializeField] private float _duration;

    private void Awake()
    {
        _buttonText = GetComponentInChildren<TMP_Text>();
        _originalColor = _buttonText.color;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        _buttonText.DOColor(_toColor, _duration)
            .SetEase(Ease.InOutSine);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        _buttonText.DOColor(_originalColor, _duration)
    .SetEase(Ease.InOutSine);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        _buttonText.DOColor(_originalColor, _duration)
        .SetEase(Ease.InOutSine);
    }

}
