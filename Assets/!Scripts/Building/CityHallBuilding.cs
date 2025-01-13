using UnityEngine;

public class CityHallBuilding : RepairableBuilding
{
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
