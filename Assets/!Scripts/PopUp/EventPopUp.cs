using UnityEngine;
using TMPro;
using DG.Tweening;

public class EventPopUp : InfoPopUp
{
    public static EventPopUp Instance;
    public TextMeshProUGUI ButtonText;

    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    private void Start()
    {
        _isDestroyable = false;
    }

    public void ShowEventPopUp(string Label, string Description, string Button)
    {
        ControllersManager.Instance.mainGameUIController.TurnOffUI();

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
        base.HidePopUp();

        if (ControllersManager.Instance.mainGameController.GameOverState != MainGameController.GameOverStateEnum.Playing)
        {
            ControllersManager.Instance.mainGameUIController.LoadMainMenu();
        }
    }
}