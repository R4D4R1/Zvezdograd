using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class RepairPopUp : InfoPopUp
{
    public static RepairPopUp Instance;

    [SerializeField] private TextMeshProUGUI _demandsText;
    [SerializeField] private TextMeshProUGUI _errorText;
    [SerializeField] private TextMeshProUGUI _applyButtonText;
    [SerializeField] private TextMeshProUGUI _denyButtonText;

    private RepairableBuilding _buildingToUse;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    public void ShowRepairPopUp(RepairableBuilding buildingToRepair)
    {
        _buildingToUse = buildingToRepair;

        _demandsText.text = $" - {_buildingToUse.PeopleToRepair} îòðÿäà ( ó âàñ {ControllersManager.Instance.peopleUnitsController.GetReadyUnits()} )\n" +
            $" - {_buildingToUse.BuildingMaterialsToRepair} ñòðîéìàòåðèàëîâ ( ó âàñ {ControllersManager.Instance.resourceController.GetBuildingMaterials()} )\n" +
            $" - {_buildingToUse.TurnsToRepair} õîäîâ";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            LabelText.DOFade(1, fadeDuration);
            DescriptionText.DOFade(1, fadeDuration);
            _demandsText.DOFade(1, fadeDuration);
            _applyButtonText.DOFade(1, fadeDuration);
            _denyButtonText.DOFade(1, fadeDuration);
        });
    }

    public override void HidePopUp()
    {
        _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration);

        ControllersManager.Instance.mainGameUIController.EnableEscapeMenuToggle();
        ControllersManager.Instance.mainGameUIController.TurnOnUI();
        ControllersManager.Instance.blurController.UnBlurBackGroundSmoothly();

        _errorText.enabled = false;

        SetTextAlpha(0);
    }

    public void RepairBuilding()
    {

        if (EnoughPeopleToReapir() && EnoughMaterialsToRepair())
        {
            HidePopUp();
            _buildingToUse.RepairBuilding();
        }
        else if (!EnoughPeopleToReapir())
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ËÞÄÅÉ";
            _errorText.enabled = true;
        }
        else
        {
            _errorText.text = "ÍÅ ÄÎÑÒÀÒÎ×ÍÎ ÐÅÑÓÐÑÎÂ";
            _errorText.enabled = true;
        }
    }

    public bool EnoughPeopleToReapir()
    {
        if (ControllersManager.Instance.peopleUnitsController.GetReadyUnits() >= _buildingToUse.PeopleToRepair)
            return true;
        else
            return false;
    }

    public bool EnoughMaterialsToRepair()
    {
        if (ControllersManager.Instance.resourceController.GetBuildingMaterials() >= _buildingToUse.BuildingMaterialsToRepair)
            return true;
        else
            return false;
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