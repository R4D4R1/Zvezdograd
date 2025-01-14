using TMPro;
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
                Debug.Log("FAILED TO SEND ARMY MATERIALS");

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
        if (!BuildingIsActive)
        {
            _turnsToWork--;
            if (_turnsToWork == 0)
            {
                BuildingIsActive = true;

                RestoreOriginalMaterials();
            }
        }
    }
}
