using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class HospitalBuildingSaveData : RepairableBuildingSaveData
{
    [JsonProperty] public int turnsToGiveMedicine;
    [JsonProperty] public int daysToGiveMedicine;
    [JsonProperty] public bool medicineWasGivenAwayInLastTwoDays;
    [JsonProperty] public bool isWorking;
    [JsonProperty] public int turnsToCreateNewUnit;
}