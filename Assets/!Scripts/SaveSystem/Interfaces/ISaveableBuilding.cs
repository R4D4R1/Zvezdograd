public interface ISaveableBuilding
{
    int BuildingId { get; }
    BuildingSaveData GetSaveData();
    void LoadFromSaveData(BuildingSaveData data);
}