public interface ISaveableBuilding
{
    int BuildingID { get; }
    BuildingSaveData GetSaveData();
    void LoadFromSaveData(BuildingSaveData data);
}