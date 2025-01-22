using UnityEngine;

public class CityHallBuilding : RepairableBuilding
{
    [field: SerializeField] public int _relationWithGoverment { get; private set; }

    [SerializeField] private int _daysLeftToSendArmyMaterialsOriginal;
    public int _daysLeftToSendArmyMaterials { get; private set; }

    [SerializeField] private int _daysLeftToRecieveGovHelpOriginal;
    private int _daysLeftToRecieveGovHelp;


    private void Start()
    {
        _daysLeftToRecieveGovHelp = _daysLeftToRecieveGovHelpOriginal;
        _daysLeftToSendArmyMaterials = _daysLeftToSendArmyMaterialsOriginal;
        ControllersManager.Instance.timeController.OnNextDayEvent += TimeController_OnNextDayEvent;
    }

    private void TimeController_OnNextDayEvent()
    {
        DayPassedForGovHelp();
    }

    protected override void TryTurnOnBuilding()
    {
        if (!BuildingIsSelactable)
        {
            _turnsToRepair--;
            if (_turnsToRepair == 0)
            {
                BuildingIsSelactable = true;

                RestoreOriginalMaterials();
            }
        }
    }

    public void DayPassedForGovHelp()
    {
        _daysLeftToRecieveGovHelp--;
        if (_daysLeftToRecieveGovHelp == 0)
        {
            _daysLeftToRecieveGovHelp = _daysLeftToRecieveGovHelpOriginal;
            RecieveHelpFromGov();
        }
    }

    public bool DayPassed()
    {
        _daysLeftToSendArmyMaterials--;

        if (_daysLeftToSendArmyMaterials == 0)
        {
            _daysLeftToSendArmyMaterials = _daysLeftToSendArmyMaterialsOriginal;

            if (_relationWithGoverment > 1)
            {
                _relationWithGoverment -= 2;
                Debug.Log("ARMY MATERIALS SENT");

                return true;
            }
            else
            {
                Debug.Log("GAME OVER");

                return false;
                // GAMEOVER
            }
        }

        return false;
    }

    private void RecieveHelpFromGov()
    {
        int foodAmount = 0;
        int medicineAmount = 0;

        if (_relationWithGoverment < 4)
        {
            foodAmount = 2;
            medicineAmount = 1;
            //Debug.Log("Supplies received: 2 food, 1 medicine (Poor relationship)");
        }
        else if (_relationWithGoverment < 8)
        {
            foodAmount = 3;
            medicineAmount = 2;
            //Debug.Log("Supplies received: 3 food, 2 medicine (Good relationship)");
        }
        else
        {
            foodAmount = 4;
            medicineAmount = 2;
            //Debug.Log("Supplies received: 4 food, 2 medicine (Excellent relationship)");
        }

        // Добавляем еду и медикаменты через контроллер ресурсов
        ControllersManager.Instance.resourceController.AddOrRemoveProvision(foodAmount);
        ControllersManager.Instance.resourceController.AddOrRemoveMedicine(medicineAmount);
    }

    public void AddRelationWithGov(int value)
    {
        _relationWithGoverment += Mathf.Clamp(Mathf.Abs(value), 0, 10);
    }

    public void ArmyMaterialsSent()
    {
        _daysLeftToSendArmyMaterials = _daysLeftToSendArmyMaterialsOriginal;
    }
}