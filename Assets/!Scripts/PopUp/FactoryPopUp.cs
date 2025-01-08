using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class FactoryPopUp : InfoPopUp
{
    private FactoryBuilding _buildingToUse;
    [SerializeField] protected TextMeshProUGUI _errorText;
    [SerializeField] protected TextMeshProUGUI _createReadyMaterialButtonText;
    [SerializeField] protected TextMeshProUGUI _creatArmyMaterialButtonText;
    [SerializeField] protected TextMeshProUGUI _denyButtonText;

    // Õ¿ «¿¬Œƒ≈ Ã€ ÃŒ∆≈Ã ƒ≈À¿“‹ Ã¿“≈–»¿À€ »« —€–‹ﬂ
    // Ã€ ÃŒ∆≈Ã —Œ«ƒ¿¬¿“‹ »« Ã¿“≈–»¿ÀŒ¬ «¿ ¿«€ Õ¿œ–»Ã≈– ◊¿—“» ¬ŒŒ–”∆≈Õ»ﬂ
    //  ÕŒœ » —Œ«ƒ¿“‹ Ã¿“≈–»¿À€ »À» ¬€œŒÀÕ»“‹ œ–» ¿«€ —Œ¬≈“¿

    // —Œ«ƒ¿“‹
    // 2 —€–‹ﬂ 1 Ã≈“¿–»¿À «¿ 3 ’Œƒ¿ + 3 Œ“ƒ€’
    // 3 —€–‹ﬂ 2 Ã¿“≈–¿À¿ «¿ 4 ’Œƒ¿ + 4 Œ“ƒ€’¿

    // ¬€œŒÀÕ»“‹ «¿ƒ¿◊” —Œ«ƒ¿“‹ » Œ“œ–¿¬»“‹ “≈’Õ» ”

    public void ShowFactoryPopUp(FactoryBuilding factoryBuilding)
    {
        _buildingToUse = factoryBuilding;

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            LabelText.DOFade(1, fadeDuration);
            DescriptionText.DOFade(1, fadeDuration);

            _errorText.DOFade(1, fadeDuration);
            _createReadyMaterialButtonText.DOFade(1, fadeDuration);
            _creatArmyMaterialButtonText.DOFade(1, fadeDuration);
            _denyButtonText.DOFade(1, fadeDuration);
        });
    }

    public override void HidePopUp()
    {
        _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration);

        ControllersManager.Instance.mainGameUIController.EnableEscapeMenuToggle();
        ControllersManager.Instance.mainGameUIController.TurnOnUI();
        ControllersManager.Instance.blurController.UnBlurBackGroundSmoothly();

        _errorText.enabled = false;

        SetTextAlpha(0);
    }

    public void CreateReadyMaterials()
    {
        if (EnoughPeopleToCreateReadyMaterials() && EnoughRawMaterialsForReadyMaterials() && EnoughSpaceToStoreReadyMaterials())
        {
            HidePopUp();
            _buildingToUse.CreateReadyMaterials();
        }
        else if(!EnoughRawMaterialsForReadyMaterials())
        {
            Debug.Log(ControllersManager.Instance.resourceController.GetReadyMaterials());
            Debug.Log(_buildingToUse.ReadyMaterialsGet);
            Debug.Log(ControllersManager.Instance.resourceController.GetMaxReadyMaterials());

            _errorText.text = "Õ≈“” —€–‹ﬂ";
            _errorText.enabled = true;
        }
        else if (!EnoughSpaceToStoreReadyMaterials())
        {
            _errorText.text = "Õ≈“” Ã≈—“¿ ƒÀﬂ Ã¿“≈–»¿ÀŒ¬";
            _errorText.enabled = true;
        }
        else if (!EnoughPeopleToCreateReadyMaterials())
        {
            _errorText.text = "Õ≈ ƒŒ—“¿“Œ◊ÕŒ Àﬁƒ≈…";
            _errorText.enabled = true;
        }
    }

    public void CreateArmySupplies()
    {
        if (EnoughPeopleToCreateArmyMaterials() && EnoughRawMaterialsForArmyMaterrials())
        {
            HidePopUp();
            _buildingToUse.CreateArmyMaterials();
        }
        else if (!EnoughRawMaterialsForArmyMaterrials())
        {
            _errorText.text = "Õ≈“” —€–‹ﬂ";
            _errorText.enabled = true;
        }
        else if (!EnoughPeopleToCreateReadyMaterials())
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

    public bool EnoughPeopleToCreateArmyMaterials()
    {
        if (ControllersManager.Instance.peopleUnitsController.GetReadyUnits() >= _buildingToUse.PeopleToCreateReadyMaterials)
            return true;
        else
            return false;
    }

    public bool EnoughPeopleToCreateReadyMaterials()
    {
        //Debug.Log(ControllersManager.Instance.peopleUnitsController.GetReadyUnits());
        //Debug.Log(_buildingToUse.PeopleToCreateArmyMaterials);

        if (ControllersManager.Instance.peopleUnitsController.GetReadyUnits() >= _buildingToUse.PeopleToCreateArmyMaterials)
            return true;
        else
            return false;
    }

    public bool EnoughRawMaterialsForReadyMaterials()
    {
        if (ControllersManager.Instance.resourceController.GetRawMaterials() > _buildingToUse.RawMaterialsToCreateReadyMaterials)
            return true;
        else
            return false;
    }

    public bool EnoughRawMaterialsForArmyMaterrials()
    {
        if (ControllersManager.Instance.resourceController.GetRawMaterials() > _buildingToUse.RawMaterialsToCreateArmyMaterials)
            return true;
        else
            return false;
    }

    protected override async void SetTextAlpha(float alpha)
    {
        await UniTask.Delay(300);

        Color labelColor = LabelText.color;
        labelColor.a = alpha;
        LabelText.color = labelColor;

        Color descriptionColor = DescriptionText.color;
        descriptionColor.a = alpha;
        DescriptionText.color = descriptionColor;

        Color demandsColor = _createReadyMaterialButtonText.color;
        demandsColor.a = alpha;
        _createReadyMaterialButtonText.color = demandsColor;

        Color applyButtonColor = _creatArmyMaterialButtonText.color;
        applyButtonColor.a = alpha;
        _creatArmyMaterialButtonText.color = applyButtonColor;

        Color denyButtonColor = _denyButtonText.color;
        denyButtonColor.a = alpha;
        _denyButtonText.color = denyButtonColor;
    }
}
