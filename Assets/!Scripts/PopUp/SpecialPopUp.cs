using DG.Tweening;
using TMPro;
using UnityEngine;

public class SpecialPopUp : InfoPopUp
{
    public TextMeshProUGUI ButtonText;

    public enum PopUpFuncs
    {
        OpenRepairMenu,
        OpenFactoryMenu,
        OpenCollectMenu
    }

    [HideInInspector]
    public RepairableBuilding RepairableBuilding;
    [HideInInspector]
    public CollectableBuilding CollectableBuilding;
    [HideInInspector]
    public FactoryBuilding FactoryBuilding;

    public PopUpFuncs CurrentFunc;

    private CollectPopUp _collectPopUp;
    private RepairPopUp _repairPopUp;
    private FactoryPopUp _factoryPopUp;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _collectPopUp = ControllersManager.Instance.selectionController.GetCollectPopUp();
        _repairPopUp = ControllersManager.Instance.selectionController.GetRepairPopUp();
        _factoryPopUp = ControllersManager.Instance.selectionController.GetFactoryPopUp();
    }

    private void OnEnable()
    {
        _bgImage.transform.localScale = Vector3.zero;
        SetTextAlpha(0);
    }

    public void UseButton()
    {
        switch (CurrentFunc)
        {
            case PopUpFuncs.OpenCollectMenu:
                _collectPopUp.ShowCollectPopUp(CollectableBuilding);
                break;
            case PopUpFuncs.OpenRepairMenu:
                _repairPopUp.ShowRepairPopUp(RepairableBuilding);
                break;
            case PopUpFuncs.OpenFactoryMenu:
                _factoryPopUp.ShowFactoryPopUp(FactoryBuilding);
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
        CurrentFunc = PopUpFuncs.OpenRepairMenu;
    }
    public void SetToCollect()
    {
        CurrentFunc = PopUpFuncs.OpenCollectMenu;
    }

    public void SetToOpenSpecialMenu()
    {
        CurrentFunc = PopUpFuncs.OpenFactoryMenu;
    }
}