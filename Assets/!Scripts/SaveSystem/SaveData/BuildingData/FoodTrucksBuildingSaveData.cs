using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class FoodTrucksBuildingSaveData : RepairableBuildingSaveData
{
    [JsonProperty] public int turnsToToGiveProvision;
    [JsonProperty] public bool isFoodGivenAwayToday;
}