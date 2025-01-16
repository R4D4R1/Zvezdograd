using DG.Tweening;
using TMPro;
using UnityEngine;

public class FoodTruckPopUp : InfoPopUp
{
    private FoodTrucksBuilding _buildingToUse;
    [SerializeField] protected TextMeshProUGUI _errorText;
    [SerializeField] protected TextMeshProUGUI _denyButtonText;

    public void ShowFoodTruckPopUp(FoodTrucksBuilding foodTrucks)
    {
        _buildingToUse = foodTrucks;

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            SetAlpha(1);
        });
    }

    public override void HidePopUp()
    {
        if (IsActive)
        {

            _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration).OnComplete(() =>
            {
                IsActive = false;
            });

            _errorText.enabled = false;

            ControllersManager.Instance.mainGameUIController.EnableEscapeMenuToggle();
            ControllersManager.Instance.mainGameUIController.TurnOnUI();

            SetAlpha(0);
        }
    }

}
