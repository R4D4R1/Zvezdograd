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
        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            LabelText.DOFade(1, fadeDuration);
            DescriptionText.DOFade(1, fadeDuration);
            ButtonText.DOFade(1, fadeDuration);
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

            LabelText.DOFade(1, fadeDuration);
            DescriptionText.DOFade(1, fadeDuration);
            ButtonText.DOFade(1, fadeDuration);
        });
    }

    public override void HidePopUp()
    {
        _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration);

        ControllersManager.Instance.mainGameUIController.EnableEscapeMenuToggle();
        ControllersManager.Instance.mainGameUIController.TurnOnUI();
        ControllersManager.Instance.blurController.UnBlurBackGroundSmoothly();

        SetTextAlpha(0);
    }
}
