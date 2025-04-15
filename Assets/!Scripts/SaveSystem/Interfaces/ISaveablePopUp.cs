public interface ISaveablePopUp
{
    int PopUpID { get; }
    PopUpSaveData SaveData();
    void LoadData(PopUpSaveData data);
}