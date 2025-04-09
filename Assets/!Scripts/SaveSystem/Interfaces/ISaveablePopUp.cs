public interface ISaveablePopUp
{
    int PopUpId { get; }
    PopUpSaveData GetSaveData();
    void LoadFromSaveData(PopUpSaveData data);
}