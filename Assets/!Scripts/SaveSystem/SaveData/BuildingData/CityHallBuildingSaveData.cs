using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class CityHallBuildingSaveData : RepairableBuildingSaveData
{
    [JsonProperty] public int relationWithGovernment;
    [JsonProperty] public int daysLeftToReceiveGovHelp;
    [JsonProperty] public int daysLeftToSendArmyMaterials;
    [JsonProperty] public bool isMaterialsSent;

    [JsonProperty] public int amountOfHelpSent;
    [JsonProperty] public int turnsToCreateNewUnit;
    [JsonProperty] public int turnsToCreateNewActionPoints;
    [JsonProperty] public bool isWorking;
    [JsonProperty] public bool isCreatingActionPoints;
}