using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class FactoryBuildingSaveData : RepairableBuildingSaveData
{
    [JsonProperty] public int turnsToCreateArmyMaterials;
    [JsonProperty] public int turnsToCreateReadyMaterials;
    [JsonProperty] public int turnsToWork;
    [JsonProperty] public bool isWorking;
    [JsonProperty] public bool createArmyIsActive;
}