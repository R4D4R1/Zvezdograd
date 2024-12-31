using DG.Tweening;
using TMPro;
using UnityEngine;

public class SpecialPopUp : InfoPopUp
{
    public TextMeshProUGUI ButtonText;

    public enum PopUpFuncs
    {
        Repair,
        OpenSpecialMenu
    }

    public RepairableBuilding BuildingToUse;
    public PopUpFuncs CurrentFunc;

    private void OnEnable()
    {
        _bgImage.transform.localScale = Vector3.zero;
        SetTextAlpha(0);
    }

    public void OpenRepairMenu()
    {
        RepairPopUp.Instance.ShowRepairPopUp(BuildingToUse);
    }

    public void OpenSpecialMenu()
    {
        if (BuildingToUse.Type == RepairableBuilding.BuildingType.Hospital)
        {
            
        }

        if (BuildingToUse.Type == RepairableBuilding.BuildingType.CityHall)
        {

        }

        if (BuildingToUse.Type == RepairableBuilding.BuildingType.Factory)
        {

        }

        if (BuildingToUse.Type == RepairableBuilding.BuildingType.FoodTrucks)
        {

        }

    }

    public void UseButton()
    {
        switch (CurrentFunc)
        {
            case PopUpFuncs.Repair:
                OpenRepairMenu();
                break;
            case PopUpFuncs.OpenSpecialMenu:
                OpenSpecialMenu();
                break;
            default:
                break;
        }

        ControllersManager.Instance.mainGameUIController.DisableEscapeMenuToggle();
        ControllersManager.Instance.blurController.BlurBackGroundSmoothly();
        ControllersManager.Instance.mainGameUIController.TurnOffUI();

        HidePopUp();
    }

    public void ShowPopUp(string Label, string Description, string Button)
    {
        LabelText.text = "";
        DescriptionText.text = "";
        ButtonText.text = "";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            LabelText.text = Label;
            DescriptionText.text = Description;
            ButtonText.text = Button;

            LabelText.DOFade(1, fadeDuration);
            DescriptionText.DOFade(1, fadeDuration);
            ButtonText.DOFade(1, fadeDuration);

        });
    }


    public void SetToRepair()
    {
        CurrentFunc = PopUpFuncs.Repair;
    }

    public void SetToOpenSpecialMenu()
    {
        CurrentFunc = PopUpFuncs.OpenSpecialMenu;
    }
}
