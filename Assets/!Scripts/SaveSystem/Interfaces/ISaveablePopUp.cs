public interface ISaveablePopUp
{
    int PopUpID { get; }
    PopUpSaveData GetSaveData();
    void LoadFromSaveData(PopUpSaveData data);
}