using DG.Tweening;
using TMPro;
using UnityEngine;

public class CollectPopUp : InfoPopUp
{

    [SerializeField] private TextMeshProUGUI _demandsText;
    [SerializeField] private TextMeshProUGUI _errorText;
    [SerializeField] private TextMeshProUGUI _buttonText;

    private CollectableBuilding _buildingToUse;

    public static CollectPopUp Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    public void ShowCollectPopUp(CollectableBuilding collectableBuilding)
    {
        // ���������� ������� ����� �������� 
        // ������� ����������
        // ������� ���E�
        // ������� �����

        _buildingToUse = collectableBuilding;

        _demandsText.text = $" - �������� {_buildingToUse.RawMaterialsLeft} ����� \n" +
            $" - �� �������� {_buildingToUse.RawMaterialsGet} ����� ( � ��� {ControllersManager.Instance.resourceController.GetRawMaterials()} )\n" +
            $" - ���������� {_buildingToUse.PeopleToCollect} ������������� ( � ��� {ControllersManager.Instance.peopleUnitsController.GetReadyUnits()} )\n" +
            $" - ������ {_buildingToUse.TurnsToCollect} �����";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            LabelText.DOFade(1, fadeDuration);
            DescriptionText.DOFade(1, fadeDuration);
            _demandsText.DOFade(1, fadeDuration);
            _buttonText.DOFade(1, fadeDuration);
        });
    }

    public void CollectBuilding()
    {

        if (EnoughPeopleToCollect() && EnoughRawMaterialsToStore() && RawMaterialsLeft())
        {
            HidePopUp();
            _buildingToUse.CollectBuilding();
        }
        else if (!EnoughPeopleToCollect())
        {
            _errorText.text = "�� ���������� �����";
            _errorText.enabled = true;
        }
        else if (!EnoughRawMaterialsToStore())
        {
            _errorText.text = "�� ���������� ����� ��� ��������";
            _errorText.enabled = true;
        }
        else
        {
            _errorText.text = "������� �����������";
            _errorText.enabled = true;
        }
    }

    public bool EnoughPeopleToCollect()
    {
        if (ControllersManager.Instance.peopleUnitsController.GetReadyUnits() >= _buildingToUse.PeopleToCollect)
            return true;
        else
            return false;
    }

    public bool EnoughRawMaterialsToStore()
    {
        if (ControllersManager.Instance.resourceController.GetRawMaterials() + _buildingToUse.RawMaterialsGet <= ControllersManager.Instance.resourceController.GetMaxRawMaterials())
            return true;
        else
            return false;
    }

    public bool RawMaterialsLeft()
    {
        if (_buildingToUse.RawMaterialsLeft > 0)
            return true;
        else
            return false;
    }


    public override void HidePopUp()
    {
        _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration);

        ControllersManager.Instance.mainGameUIController.EnableEscapeMenuToggle();
        ControllersManager.Instance.mainGameUIController.TurnOnUI();
        ControllersManager.Instance.blurController.UnBlurBackGroundSmoothly();

        SetTextAlpha(0);
    }

    protected override void SetTextAlpha(float alpha)
    {
        Color labelColor = LabelText.color;
        labelColor.a = alpha;
        LabelText.color = labelColor;

        Color descriptionColor = DescriptionText.color;
        descriptionColor.a = alpha;
        DescriptionText.color = descriptionColor;

        Color demandsColor = _demandsText.color;
        demandsColor.a = alpha;
        _demandsText.color = demandsColor;

        Color buttonColor = _buttonText.color;
        buttonColor.a = alpha;
        _buttonText.color = buttonColor;
    }
}
