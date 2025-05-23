using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class EnoughPopUp : FullScreenPopUp
{
    [FormerlySerializedAs("_errorText")] [SerializeField] protected TextMeshProUGUI errorText;
    protected int PopUpId { get; private set; }

    public virtual void Init()
    {
        HideError();
        GenerateOrLoadPopUpId();
    }
    
    private void GenerateOrLoadPopUpId()
    {
        string uniqueKey = $"PopUp_{gameObject.name}";

        if (PlayerPrefs.HasKey(uniqueKey))
        {
            PopUpId = PlayerPrefs.GetInt(uniqueKey);
        }
        else
        {
            PopUpId = Random.Range(100000, 999999);
            PlayerPrefs.SetInt(uniqueKey, PopUpId);
            PlayerPrefs.Save();
        }
    }

    
    protected bool HasEnoughPeople(int requiredPeople)
    {
        if (_peopleUnitsController.ReadyUnits.Count >= requiredPeople)
            return true;

        ShowError("Недостаточно подразделений");
        return false;
    }

    protected bool HasEnoughResources(ResourceModel.ResourceType resourceType, int requiredAmount)
    {
        if (_resourceViewModel.GetResourceValue(resourceType) >= requiredAmount)
            return true;

        ShowError("Недостаточно ресурсов");
        return false;
    }
    
    protected bool HasEnoughSpaceForResources(ResourceModel.ResourceType resourceType, int materialsToGet)
    {
        if (_resourceViewModel.GetResourceValue(resourceType) + materialsToGet <= _resourceViewModel.GetMaxResourceValue(resourceType))
            return true;
        
        ShowError("Недостаточно места для ресурсов");
        return false;
    }

    protected bool CanUseActionPoint()
    {
        if (_timeController.OnActionPointUsed())
            return true;

        ShowError("Недостаточно очков действий");
        return false;
    }

    protected void ShowError(string message)
    {
        errorText.text = message;
        errorText.enabled = true;
    }

    private void HideError()
    {
        errorText.enabled = false;
    }

    public override void HidePopUp()
    {
        base.HidePopUp();
        HideError();
    }
}
