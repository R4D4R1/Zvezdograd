public interface ISaveableBuilding
{
    int BuildingID { get; }
    BuildingSaveData SaveData();
    void LoadData(BuildingSaveData data);
}