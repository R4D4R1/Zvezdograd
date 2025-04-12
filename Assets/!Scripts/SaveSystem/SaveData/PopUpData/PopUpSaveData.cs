using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class PopUpSaveData
{
    [JsonProperty] public int popUpID;
}