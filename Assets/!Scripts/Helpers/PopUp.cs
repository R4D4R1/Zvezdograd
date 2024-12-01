using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class PopUp : MonoBehaviour
{
    public enum PopUpFuncs
    {
        Repair,
        OpenSpecialMenu
    }

    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI DescriptionText;

    [SerializeField] private Image _bgImage;
    [SerializeField] private float scaleDuration = 0.5f; // Duration of the scale animation when opening
    [SerializeField] private float fadeDuration = 0.5f;  // Duration of the fade animation when opening
    [SerializeField] private float scaleDownDuration = 0.2f; // Duration of the scale animation when closing

    public RepairableBuilding BuildingToUse;
    public PopUpFuncs CurrentFunc;

    private void OnEnable()
    {
        _bgImage.transform.localScale = Vector3.zero;
        SetTextAlpha(0);
        ShowPopUp();
    }

    public void ShowPopUp()
    {
        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            LabelText.DOFade(1, fadeDuration);
            DescriptionText.DOFade(1, fadeDuration);
        });
    }

    public void HidePopUp()
    {
        _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration).OnComplete(() =>
        {
            Destroy(gameObject); // Remove the popup after the animation completes
        });
    }

    private void SetTextAlpha(float alpha)
    {
        Color labelColor = LabelText.color;
        labelColor.a = alpha;
        LabelText.color = labelColor;

        Color descriptionColor = DescriptionText.color;
        descriptionColor.a = alpha;
        DescriptionText.color = descriptionColor;
    }

    public void RepairBuilding()
    {
        BuildingToUse.RepairBuilding();

    }

    public void OpenSpecialMenu()
    {
        // OPEN MENU 
        // Implementation for opening a special menu
    }

    public void UseButton()
    {
        switch (CurrentFunc)
        {
            case PopUpFuncs.Repair:
                RepairBuilding();
                break;
            case PopUpFuncs.OpenSpecialMenu:
                OpenSpecialMenu();
                break;
            default:
                break;
        }
        HidePopUp();
    }

    public void SetToRepair()
    {
        CurrentFunc = PopUpFuncs.Repair;
    }

    public void SetToOpenSpecialMenu()
    {
        CurrentFunc = PopUpFuncs.OpenSpecialMenu;
    }
}
