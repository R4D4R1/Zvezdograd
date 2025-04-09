using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class RepairableBuildingSaveData : BuildingSaveData
{
    [JsonProperty] public int turnsToRepair;
    [JsonProperty] public RepairableBuilding.State currentState;
}