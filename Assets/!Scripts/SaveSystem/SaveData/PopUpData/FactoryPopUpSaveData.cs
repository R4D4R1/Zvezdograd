using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class FactoryPopUpSaveData : PopUpSaveData
{
    [JsonProperty] public bool isCreatingArmyMaterials;
}