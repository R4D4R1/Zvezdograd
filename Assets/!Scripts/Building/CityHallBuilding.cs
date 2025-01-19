using UnityEngine;

public class CityHallBuilding : RepairableBuilding
{
    [field: SerializeField] public int _daysLeftToSendArmyMaterialsOriginal { get; private set; } = 3;

    [field: SerializeField] public int _relationWithGoverment { get; private set; }

    public int _daysLeftToSendArmyMaterials { get; private set; }

    private void Start()
    {
        _daysLeftToSendArmyMaterials = _daysLeftToSendArmyMaterialsOriginal;
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

    public void AddRelationWithGov(int value)
    {
        _relationWithGoverment += Mathf.Clamp(Mathf.Abs(value), 0, 10);
    }
    public void ArmyMaterialsSent()
    {
        _daysLeftToSendArmyMaterials = _daysLeftToSendArmyMaterialsOriginal;
    }
}
