using DG.Tweening;
using TMPro;
using UnityEngine;
using UniRx;

public class HospitalPopUp : QuestPopUp
{
    [SerializeField] private TextMeshProUGUI _medicineTimerText;

    public override void Init()
    {
        base.Init();

        SetButtonState(true);
        UpdateMedicineTimerText();

        _controllersManager.TimeController.OnNextDayEvent
            .Subscribe(_ => OnNextDayEvent())
            .AddTo(this);
    }

    private void OnNextDayEvent()
    {
        if (_controllersManager.BuildingController.GetHospitalBuilding().MedicineWasGiven())
        {
            activeBtn.SetActive(true);
            inactiveBtn.SetActive(false);
        }

        UpdateMedicineTimerText();
    }

    public void ShowHospitalPopUp()
    {
        transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;
            SetAlpha(1);
        });
    }

    public void GiveAwayMedicine()
    {
        if (CanGiveAwayMedicine())
        {
            SetButtonState(false);
            _controllersManager.BuildingController.GetHospitalBuilding().SendPeopleToGiveMedicine();
        }
    }

    private bool CanGiveAwayMedicine()
    {
        return HasEnoughPeople(_controllersManager.BuildingController.GetHospitalBuilding().PeopleToGiveMedicine) &&
               EnoughMedicineToGiveAway() &&
               CanUseActionPoint();
    }

    private void UpdateMedicineTimerText()
    {
        _medicineTimerText.text = "Крайний срок отправки мед. помощи - " +
            _controllersManager.BuildingController.GetHospitalBuilding().DaysToGiveMedicine.ToString() + " дн.";
    }

    private bool EnoughMedicineToGiveAway()
    {
        return HasEnoughResources(ResourceModel.ResourceType.Medicine,
            _controllersManager.BuildingController.GetHospitalBuilding().MedicineToGive);
    }
}
