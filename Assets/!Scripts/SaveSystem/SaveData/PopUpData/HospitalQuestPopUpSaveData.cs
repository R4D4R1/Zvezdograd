using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class HospitalQuestPopUpSaveData : QuestPopUpSaveData
{
    [JsonProperty] public bool IsHealing;
}