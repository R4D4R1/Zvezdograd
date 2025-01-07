using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButtonRecolorText : CustomButtonBase
{
    private TMP_Text _buttonText;
    [SerializeField] private Color _originalColor;
    [SerializeField] private Color _toColor;
    [SerializeField] private float _duration;
    [SerializeField] private bool _turnBackToOriginalColor = true;

    private void Awake()
    {
        _buttonText = GetComponentInChildren<TMP_Text>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (_turnBackToOriginalColor)
        {
            _buttonText.DOColor(_toColor, _duration)
            .SetEase(Ease.InOutSine);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (_turnBackToOriginalColor)
        {
            _buttonText.DOColor(_originalColor, _duration)
            .SetEase(Ease.InOutSine);
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (_turnBackToOriginalColor)
        {
            _buttonText.DOColor(_originalColor, _duration)
            .SetEase(Ease.InOutSine);
        }
    }
}
