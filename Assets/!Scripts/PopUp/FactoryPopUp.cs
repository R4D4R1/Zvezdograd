using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class FactoryPopUp : EnoughPeoplePopUp
{
    private FactoryBuilding _buildingToUse;
    [SerializeField] protected TextMeshProUGUI _createReadyMaterialButtonText;
    [SerializeField] protected TextMeshProUGUI _creatArmyMaterialButtonText;

    // Õ¿ «¿¬Œƒ≈ Ã€ ÃŒ∆≈Ã ƒ≈À¿“‹ Ã¿“≈–»¿À€ »« —€–‹ﬂ
    // Ã€ ÃŒ∆≈Ã —Œ«ƒ¿¬¿“‹ »« Ã¿“≈–»¿ÀŒ¬ «¿ ¿«€ Õ¿œ–»Ã≈– ◊¿—“» ¬ŒŒ–”∆≈Õ»ﬂ
    //  ÕŒœ » —Œ«ƒ¿“‹ Ã¿“≈–»¿À€ »À» ¬€œŒÀÕ»“‹ œ–» ¿«€ —Œ¬≈“¿

    // —Œ«ƒ¿“‹
    // 2 —€–‹ﬂ 1 Ã≈“¿–»¿À «¿ 3 ’Œƒ¿ + 3 Œ“ƒ€’
    // 3 —€–‹ﬂ 2 Ã¿“≈–¿À¿ «¿ 4 ’Œƒ¿ + 4 Œ“ƒ€’¿

    // ¬€œŒÀÕ»“‹ «¿ƒ¿◊” —Œ«ƒ¿“‹ » Œ“œ–¿¬»“‹ “≈’Õ» ”

    private void Start()
    {
        _errorText.enabled = false;
        _isDestroyable = false;
    }
    public void ShowFactoryPopUp(FactoryBuilding factoryBuilding)
    {
        _buildingToUse = factoryBuilding;

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            IsActive = true;

            SetAlpha(1);
        });
    }

    public void CreateReadyMaterials()
    {
        if (EnoughPeopleTo(_buildingToUse.PeopleToCreateReadyMaterials) && EnoughRawMaterialsForReadyMaterials() && EnoughSpaceToStoreReadyMaterials())
        {
            HidePopUp();
            _buildingToUse.CreateReadyMaterials();
        }
        else if (!EnoughRawMaterialsForReadyMaterials())
        {
            _errorText.text = "Õ≈“” —€–‹ﬂ";
            _errorText.enabled = true;
        }
        else if (!EnoughSpaceToStoreReadyMaterials())
        {
            _errorText.text = "Õ≈“” Ã≈—“¿ ƒÀﬂ Ã¿“≈–»¿ÀŒ¬";
            _errorText.enabled = true;
        }
        else if (!EnoughPeopleTo(_buildingToUse.PeopleToCreateReadyMaterials))
        {
            _errorText.text = "Õ≈ ƒŒ—“¿“Œ◊ÕŒ Àﬁƒ≈…";
            _errorText.enabled = true;
        }
    }

    public void CreateArmySupplies()
    {
        if (EnoughPeopleTo(_buildingToUse.PeopleToCreateArmyMaterials) && EnoughRawMaterialsForArmyMaterrials())
        {
            HidePopUp();
            _buildingToUse.CreateArmyMaterials();
        }
        else if (!EnoughRawMaterialsForArmyMaterrials())
        {
            _errorText.text = "Õ≈“” —€–‹ﬂ";
            _errorText.enabled = true;
        }
        else if (!EnoughPeopleTo(_buildingToUse.PeopleToCreateArmyMaterials))
        {
            _errorText.text = "Õ≈ ƒŒ—“¿“Œ◊ÕŒ Àﬁƒ≈…";
            _errorText.enabled = true;
        }
    }

    public bool EnoughSpaceToStoreReadyMaterials()
    {
        if (ControllersManager.Instance.resourceController.GetReadyMaterials() + _buildingToUse.ReadyMaterialsGet < ControllersManager.Instance.resourceController.GetMaxReadyMaterials())
            return true;
        else
            return false;
    }

    public bool EnoughRawMaterialsForReadyMaterials()
    {
        if (ControllersManager.Instance.resourceController.GetRawMaterials() >= _buildingToUse.RawMaterialsToCreateReadyMaterials)
            return true;
        else
            return false;
    }

    public bool EnoughRawMaterialsForArmyMaterrials()
    {
        if (ControllersManager.Instance.resourceController.GetRawMaterials() >= _buildingToUse.RawMaterialsToCreateArmyMaterials)
            return true;
        else
            return false;
    }
}
