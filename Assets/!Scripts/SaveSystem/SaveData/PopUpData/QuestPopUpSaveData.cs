using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class QuestPopUpSaveData:PopUpSaveData
{
    [JsonProperty] public bool isBtnActive;
}