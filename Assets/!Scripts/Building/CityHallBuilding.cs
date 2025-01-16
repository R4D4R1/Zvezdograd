using UnityEngine;

public class CityHallBuilding : RepairableBuilding
{
    public static CityHallBuilding Instance { get; private set; }
    [field: SerializeField] public int _daysLeftToSendArmyMaterialsOriginal { get; private set; } = 3;

    [field: SerializeField] public int _relationWithGoverment { get; private set; }

    public int _daysLeftToSendArmyMaterials { get; private set; }

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

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
