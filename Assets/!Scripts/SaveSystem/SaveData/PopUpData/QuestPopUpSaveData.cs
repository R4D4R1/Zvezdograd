using Newtonsoft.Json;
using System.Collections.Generic;

[JsonObject(MemberSerialization.OptIn)]
public class QuestPopUpSaveData:PopUpSaveData
{
    [JsonProperty] public bool isBtnActive;
    [JsonProperty] public List<QuestSaveData> savedQuests = new();
}