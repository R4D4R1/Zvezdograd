using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class RepairPopUp : MonoBehaviour
{
    public static RepairPopUp Instance;

    [SerializeField] private TextMeshProUGUI _labelText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _demandsText;
    [SerializeField] private TextMeshProUGUI _errorText;

    [SerializeField] private Image _bgImage;
    [SerializeField] private float scaleDuration = 0.5f; // Duration of the scale animation when opening
    [SerializeField] private float fadeDuration = 0.5f;  // Duration of the fade animation when opening
    [SerializeField] private float scaleDownDuration = 0.2f; // Duration of the scale animation when closing

    [HideInInspector]
    public RepairableBuilding BuildingToUse;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    public void ShowRepairPopUp(RepairableBuilding buildingToRepair)
    {
        BuildingToUse = buildingToRepair;

        _demandsText.text = $" - {BuildingToUse.PeopleToRepair} отряда ( у вас {ControllersManager.Instance.peopleUnitsController.GetReadyUnits()} )\n" +
            $" - {BuildingToUse.BuildingMaterialsToRepair} стройматериалов ( у вас {ControllersManager.Instance.resourceController.GetBuildingMaterials()} )\n" +
            $" - {BuildingToUse.TurnsToRepair} ходов";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            _labelText.DOFade(1, fadeDuration);
            _descriptionText.DOFade(1, fadeDuration);
            _demandsText.DOFade(1, fadeDuration);
        });


    }

    public void HidePopUp()
    {
        _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration);

        ControllersManager.Instance.mainGameUIController.EnableEscapeMenuToggle();
        ControllersManager.Instance.mainGameUIController.TurnOnUI();
        ControllersManager.Instance.blurController.UnBlurBackGroundSmoothly();
    }

    public void RepairBuilding()
    {

        if (CheckIfEnoughResources())
        {
            HidePopUp();
            BuildingToUse.RepairBuilding();
        }
        else
        {
            _errorText.enabled = true;
        }
    }

    public bool CheckIfEnoughResources()
    {
        if(ControllersManager.Instance.peopleUnitsController.GetReadyUnits()>=BuildingToUse.PeopleToRepair&&
            ControllersManager.Instance.resourceController.GetBuildingMaterials()>= BuildingToUse.BuildingMaterialsToRepair)
            return true;
        else
            return false;
    }
}
