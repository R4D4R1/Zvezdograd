using System;
using System.Collections.Generic;

[Serializable]
public class CombinedGameData
{
    public GameData mainGameData;
    public List<PopUpSaveData> popUpSaveData = new();
    public List<BuildingSaveData> buildingSaveData = new();
}