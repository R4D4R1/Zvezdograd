using UnityEngine;

public class CityHallBuilding : RepairableBuilding
{
    [field: SerializeField] public int RelationWithGoverment { get; private set; }

    [SerializeField] private int _daysLeftToSendArmyMaterialsOriginal;
    public int DaysLeftToSendArmyMaterials { get; private set; }

    [SerializeField] private int _daysLeftToRecieveGovHelpOriginal;
    public int DaysLeftToRecieveGovHelp { get; private set; }


    private void Start()
    {
        DaysLeftToRecieveGovHelp = _daysLeftToRecieveGovHelpOriginal;
        DaysLeftToSendArmyMaterials = _daysLeftToSendArmyMaterialsOriginal;
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
        DaysLeftToRecieveGovHelp--;
        if (DaysLeftToRecieveGovHelp == 0)
        {
            DaysLeftToRecieveGovHelp = _daysLeftToRecieveGovHelpOriginal;
            RecieveHelpFromGov();
        }
    }

    public bool DayPassed()
    {
        DaysLeftToSendArmyMaterials--;

        if (DaysLeftToSendArmyMaterials == 0)
        {
            DaysLeftToSendArmyMaterials = _daysLeftToSendArmyMaterialsOriginal;

            if (RelationWithGoverment > 1)
            {
                RelationWithGoverment -= 2;
                Debug.Log("DID NOT SENT ARMY MATERIALS");

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

        if (RelationWithGoverment < 4)
        {
            foodAmount = 2;
            medicineAmount = 1;
            //Debug.Log("Supplies received: 2 food, 1 medicine (Poor relationship)");
        }
        else if (RelationWithGoverment < 8)
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
        RelationWithGoverment += Mathf.Clamp(Mathf.Abs(value), 0, 10);
    }

    public void ArmyMaterialsSent()
    {
        DaysLeftToSendArmyMaterials = _daysLeftToSendArmyMaterialsOriginal;
    }
}