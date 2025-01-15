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
        if (IsActive)
        {
            IsActive = false;

            _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration);

            _errorText.enabled = false;

            ControllersManager.Instance.mainGameUIController.EnableEscapeMenuToggle();
            ControllersManager.Instance.mainGameUIController.TurnOnUI();

            SetAlpha(0);
        }
    }
}
