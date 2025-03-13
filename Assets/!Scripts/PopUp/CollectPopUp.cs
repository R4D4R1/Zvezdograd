using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CollectPopUp : EnoughPopUp
{
    private CollectableBuilding buildingToUse;

    [SerializeField] private TextMeshProUGUI demandsText;

    private void Start()
    {
        _errorText.enabled = false;
    }

    public void ShowCollectPopUp(CollectableBuilding collectableBuilding)
    {
        buildingToUse = collectableBuilding;

        demandsText.text = $" - �������� {buildingToUse.RawMaterialsLeft} �����\n" +
                           $" - �� �������� {buildingToUse.RawMaterialsGet} ����� (� ��� {_resourceViewModel.RawMaterials.Value})\n" +
                           $" - ���������� {buildingToUse.PeopleToCollect} ������������� (� ��� {_controllersManager.PeopleUnitsController.ReadyUnits.Count})\n" +
                           $" - ������ {buildingToUse.TurnsToCollect} �����\n" +
                           $" - ������������� ����� �������� {buildingToUse.TurnsToRest} �����";

        transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;
            SetAlpha(1);
        });
    }

    public void CollectBuilding()
    {
        if (CheckForEnoughPeople(buildingToUse.PeopleToCollect) && EnoughRawMaterialsToStore() && EnoughRawMaterialsInBuilding())
        {
            if (!CanUseActionPoint())
                return;

            HidePopUp();
            buildingToUse.CollectBuilding();
        }
        else
        {
            ShowErrorMessage();
        }
    }

    private void ShowErrorMessage()
    {
        if (!CheckForEnoughPeople(buildingToUse.PeopleToCollect))
        {
            _errorText.text = "�� ���������� �����";
        }
        else if (!EnoughRawMaterialsToStore())
        {
            _errorText.text = "�� ���������� ����� ��� ��������";
        }
        else
        {
            _errorText.text = "������� �����������";
        }
        _errorText.enabled = true;
    }

    public bool EnoughRawMaterialsToStore()
    {
        return _resourceViewModel.RawMaterials.Value + buildingToUse.RawMaterialsGet <= _resourceViewModel.Model.MaxRawMaterials;
    }

    public bool EnoughRawMaterialsInBuilding()
    {
        return buildingToUse.RawMaterialsLeft > 0;
    }
}
