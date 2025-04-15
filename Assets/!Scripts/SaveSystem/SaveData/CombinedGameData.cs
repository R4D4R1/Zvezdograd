using Newtonsoft.Json;
using System.Collections.Generic;

[JsonObject(MemberSerialization.OptIn)]
public class CombinedGameData
{
    [JsonProperty] public GameData mainGameData;
    [JsonProperty] public List<PopUpSaveData> popUpSaveData = new();
    [JsonProperty] public List<BuildingSaveData> buildingSaveData = new();
}