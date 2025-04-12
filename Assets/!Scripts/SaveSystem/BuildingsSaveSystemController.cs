using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;
using Zenject;

public class BuildingsSaveSystemController : MonoBehaviour
{
    private const string SaveFileName = "buildings_save.json";

    private readonly JsonSerializerSettings _settings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.Indented
    };

    [Inject] private BuildingsController _buildingsController;

    public void SaveBuildings()
    {
        var saveDataDict = new Dictionary<string, List<BuildingSaveData>>();
        var allBuildings = _buildingsController.AllBuildings;

        foreach (var obj in allBuildings)
        {
            if (obj is ISaveableBuilding saveable)
            {
                var data = saveable.GetSaveData();
                var typeName = obj.GetType().Name;

                if (!saveDataDict.ContainsKey(typeName))
                {
                    saveDataDict[typeName] = new List<BuildingSaveData>();
                }

                saveDataDict[typeName].Add(data);
            }
        }

        var wrapper = new SaveDataWrapper { dataByType = saveDataDict };
        var json = JsonConvert.SerializeObject(wrapper, _settings);
        File.WriteAllText(GetSavePath(), json);

        Debug.Log($"Здания сохранены: {GetSavePath()}");
    }

    public void LoadBuildings()
    {
        if (!File.Exists(GetSavePath()))
        {
            Debug.LogWarning("Файл сохранения зданий не найден!");
            return;
        }

        var json = File.ReadAllText(GetSavePath());
        var wrapper = JsonConvert.DeserializeObject<SaveDataWrapper>(json, _settings);
        var allBuildings = _buildingsController.AllBuildings;

        foreach (var obj in allBuildings)
        {
            if (obj is ISaveableBuilding saveable)
            {
                var typeName = obj.GetType().Name;

                if (wrapper.dataByType.TryGetValue(typeName, out var buildingList))
                {
                    var data = buildingList.Find(d => d.buildingID == saveable.BuildingID);
                    if (data != null)
                    {
                        saveable.LoadFromSaveData(data);
                    }
                }
            }
        }
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    [Serializable]
    private class SaveDataWrapper
    {
        public Dictionary<string, List<BuildingSaveData>> dataByType = new();
    }
}
