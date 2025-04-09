using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class BuildingSaveData
{
    [JsonProperty] public int buildingId;
    [JsonProperty] public bool buildingIsSelectable;
}