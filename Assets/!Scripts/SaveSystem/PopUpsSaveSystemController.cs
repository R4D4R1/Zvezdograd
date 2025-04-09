using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;
using Zenject;

public class PopUpsSaveSystemController : MonoBehaviour
{
    private const string SaveFileName = "popups_save.json";

    private readonly JsonSerializerSettings _settings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.Indented
    };

    [Inject] private PopUpsController _popUpsController;

    public void SavePopUps()
    {
        var dataByType = new Dictionary<string, List<PopUpSaveData>>();
        var allPopUps = _popUpsController.AllPopUps;

        foreach (var obj in allPopUps)
        {
            if (obj is ISaveablePopUp saveable)
            {
                string typeName = saveable.GetType().Name;

                if (!dataByType.ContainsKey(typeName))
                {
                    dataByType[typeName] = new List<PopUpSaveData>();
                }

                dataByType[typeName].Add(saveable.GetSaveData());
            }
        }

        var wrapper = new SaveDataWrapper { dataByType = dataByType };
        var json = JsonConvert.SerializeObject(wrapper, _settings);
        File.WriteAllText(GetSavePath(), json);

        Debug.Log($"Попапы сохранены: {GetSavePath()}");
    }

    public void LoadPopUps()
    {
        if (!File.Exists(GetSavePath()))
        {
            Debug.LogWarning("Файл сохранения попапов не найден!");
            return;
        }

        var json = File.ReadAllText(GetSavePath());
        var wrapper = JsonConvert.DeserializeObject<SaveDataWrapper>(json, _settings);
        var allPopUps = _popUpsController.AllPopUps;

        foreach (var obj in allPopUps)
        {
            if (obj is ISaveablePopUp saveable)
            {
                var typeName = saveable.GetType().Name;

                if (wrapper.dataByType.TryGetValue(typeName, out var list))
                {
                    var data = list.Find(d => d.popUpId == saveable.PopUpId);
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
        public Dictionary<string, List<PopUpSaveData>> dataByType = new();
    }
}
