using DG.Tweening;
using TMPro;
using UnityEngine;

public class SpecialPopUp : InfoPopUp
{
    public TextMeshProUGUI ButtonText;

    public enum PopUpFuncs
    {
        Repair,
        OpenSpecialMenu,
        Collect
    }

    [HideInInspector]
    public RepairableBuilding RepairableBuilding;
    [HideInInspector]
    public CollectableBuilding CollectableBuilding;

    public PopUpFuncs CurrentFunc;

    private CollectPopUp _collectPopUp;
    private RepairPopUp _repairPopUp;
    

    private void OnEnable()
    {
        _bgImage.transform.localScale = Vector3.zero;
        SetTextAlpha(0);
    }

    public void OpenSpecialMenu()
    {
        if (RepairableBuilding.Type == RepairableBuilding.BuildingType.Hospital)
        {

        }

        if (RepairableBuilding.Type == RepairableBuilding.BuildingType.CityHall)
        {

        }

        if (RepairableBuilding.Type == RepairableBuilding.BuildingType.Factory)
        {

        }

        if (RepairableBuilding.Type == RepairableBuilding.BuildingType.FoodTrucks)
        {

        }
    }

    public void UseButton()
    {
        switch (CurrentFunc)
        {
            case PopUpFuncs.Collect:
                _collectPopUp.ShowCollectPopUp(CollectableBuilding);
                break;
            case PopUpFuncs.Repair:
                _repairPopUp.ShowRepairPopUp(RepairableBuilding);
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

    public void InitializeRepair(RepairPopUp repairPopUp)
    {
        _repairPopUp = repairPopUp;
    }

    public void InitializeCollect(CollectPopUp collectPopUp)
    {
        _collectPopUp = collectPopUp;
    }

    public void InitializeSpecial()
    {

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

    public void SetToCollect()
    {
        CurrentFunc = PopUpFuncs.Collect;
    }
}