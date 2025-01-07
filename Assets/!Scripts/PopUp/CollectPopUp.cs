using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CollectPopUp : InfoPopUp
{
    [SerializeField] private TextMeshProUGUI _demandsText;
    [SerializeField] private TextMeshProUGUI _errorText;
    [SerializeField] private TextMeshProUGUI _applyButtonText;
    [SerializeField] private TextMeshProUGUI _denyButtonText;

    private CollectableBuilding _buildingToUse;


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
            $" - ������ {_buildingToUse.TurnsToCollect} �����\n" +
            $" - ������������� ����� �������� {_buildingToUse.TurnsToRest} �����";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            LabelText.DOFade(1, fadeDuration);
            DescriptionText.DOFade(1, fadeDuration);
            _demandsText.DOFade(1, fadeDuration);
            _applyButtonText.DOFade(1, fadeDuration);
            _denyButtonText.DOFade(1, fadeDuration);
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