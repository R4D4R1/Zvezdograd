using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class CollectableBuildingSaveData : BuildingSaveData
{
    [JsonProperty] public int rawMaterialsLeft;
    [JsonProperty] public int turnsToCollect;
    [JsonProperty] public int turnsToWork;
}