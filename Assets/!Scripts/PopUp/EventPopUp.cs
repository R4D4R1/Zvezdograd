using UnityEngine;
using TMPro;
using DG.Tweening;

public class EventPopUp : InfoPopUp
{
    public static EventPopUp Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    public TextMeshProUGUI ButtonText;

    public override void ShowPopUp()
    {
        IsActive = true;

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            SetAlpha(1f);
        });
    }

    public void ShowEventPopUp(string Label, string Description, string Button)
    {
        ControllersManager.Instance.mainGameUIController.TurnOffUI();
        ControllersManager.Instance.mainGameUIController.DisableEscapeMenuToggle();
        ControllersManager.Instance.blurController.BlurBackGroundSmoothly();

        LabelText.text = "";
        DescriptionText.text = "";
        ButtonText.text = "";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            LabelText.text = Label;
            DescriptionText.text = Description;
            ButtonText.text = Button;

            IsActive = true;

            SetAlpha(1);
        });
    }

    public override void HidePopUp()
    {
        if (IsActive)
        {
            IsActive = false;

            _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration);

            ControllersManager.Instance.mainGameUIController.EnableEscapeMenuToggle();
            ControllersManager.Instance.mainGameUIController.TurnOnUI();

            SetAlpha(0);
        }
    }
}