using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class QuestSaveData
{
    [JsonProperty] public string QuestName;
    [JsonProperty] public int DeadlineInDays;
    [JsonProperty] public int UnitSize;
    [JsonProperty] public int TurnsToWork;
    [JsonProperty] public int TurnsToRest;
    [JsonProperty] public int MaterialsToGet;
    [JsonProperty] public int MaterialsToLose;
    [JsonProperty] public int StabilityToGet;
    [JsonProperty] public int StabilityToLose;
    [JsonProperty] public int RelationshipWithGovToGet;
    [JsonProperty] public int RelationshipWithGovToLose;
    [JsonProperty] public string BuildingType;
}
