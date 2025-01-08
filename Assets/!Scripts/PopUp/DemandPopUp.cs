using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DemandPopUp : InfoPopUp
{
    [SerializeField] protected TextMeshProUGUI _demandsText;
    [SerializeField] protected TextMeshProUGUI _errorText;
    [SerializeField] protected TextMeshProUGUI _applyButtonText;
    [SerializeField] protected TextMeshProUGUI _denyButtonText;

    public override void HidePopUp()
    {
        _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration);

        ControllersManager.Instance.mainGameUIController.EnableEscapeMenuToggle();
        ControllersManager.Instance.mainGameUIController.TurnOnUI();
        ControllersManager.Instance.blurController.UnBlurBackGroundSmoothly();

        _errorText.enabled = false;

        SetTextAlpha(0);
    }

    protected override async void SetTextAlpha(float alpha)
    {
        await UniTask.Delay(300);

        Color labelColor = LabelText.color;
        labelColor.a = alpha;
        LabelText.color = labelColor;

        Color descriptionColor = DescriptionText.color;
        descriptionColor.a = alpha;
        DescriptionText.color = descriptionColor;

        Color demandsColor = _demandsText.color;
        demandsColor.a = alpha;
        _demandsText.color = demandsColor;

        Color applyButtonColor = _applyButtonText.color;
        applyButtonColor.a = alpha;
        _applyButtonText.color = applyButtonColor;

        Color denyButtonColor = _denyButtonText.color;
        denyButtonColor.a = alpha;
        _denyButtonText.color = denyButtonColor;
    }
}
