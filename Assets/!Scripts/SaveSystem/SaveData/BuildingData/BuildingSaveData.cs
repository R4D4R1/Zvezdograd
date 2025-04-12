using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class BuildingSaveData
{
    [JsonProperty] public int buildingID;
    [JsonProperty] public bool buildingIsSelectable;
}