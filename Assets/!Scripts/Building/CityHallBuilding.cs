using System;
using UnityEngine;

public class CityHallBuilding : RepairableBuilding
{
    [SerializeField] private int _daysLeftToSendArmyMaterialsOriginal;
    [SerializeField] private int _daysLeftToRecieveGovHelpOriginal;

    
    [Range(0,10), SerializeField] private int _amountOfHelpNeededToSent;
    private int _amountOfHelpSent = 0;

    // SAVE DATA
    [Range(0f, 10f)]
    [field: SerializeField] public int RelationWithGoverment { get; private set; }
    public int DaysLeftToRecieveGovHelp { get; private set; }
    public int DaysLeftToSendArmyMaterials { get; private set; }
    public bool IsMaterialsSent { get; private set; }


    //public event Action ArmyMaterialsWereSent;
    //public event Action UpdateArmyRequirement;

    // «дание совета

    // ƒелать поставки вооружени€ с завода - написан срок
    // «а непоставк у в срок - минус 2 очка / за поставку плюс 2

    //передать дл€ государства медикаменты
    //передать стройматериалы
    //передать провизию

    private void Start()
    {
        InitializeTimers();
        SubscribeToEvents();
    }

    private void InitializeTimers()
    {
        DaysLeftToRecieveGovHelp = _daysLeftToRecieveGovHelpOriginal;
        DaysLeftToSendArmyMaterials = _daysLeftToSendArmyMaterialsOriginal;
    }

    private void SubscribeToEvents()
    {
        ControllersManager.Instance.timeController.OnNextDayEvent += TimeController_OnNextDayEvent;
    }

    private void TimeController_OnNextDayEvent()
    {
        DayPassedForGovHelp();
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

            if (IsMaterialsSent)
            {
                ControllersManager.Instance.popUpsController.FactoryPopUp.UpdateCreateArmyButtonState();

                IsMaterialsSent = false;
            }
            else 
            {
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

        // ƒобавл€ем еду и медикаменты через контроллер ресурсов
        ControllersManager.Instance.resourceController.AddOrRemoveProvision(foodAmount);
        ControllersManager.Instance.resourceController.AddOrRemoveMedicine(medicineAmount);
    }
    public void ArmyMaterialsSent()
    {
        _amountOfHelpSent++;
        IsMaterialsSent = true;
        AddRelationWithGov(2);

        if (_amountOfHelpSent >= _amountOfHelpNeededToSent)
        {
            ControllersManager.Instance.mainGameController.GameWin();
        }
    }

    public void AddRelationWithGov(int value)
    {
        RelationWithGoverment += Mathf.Clamp(Mathf.Abs(value), 0, 10);
    }

}