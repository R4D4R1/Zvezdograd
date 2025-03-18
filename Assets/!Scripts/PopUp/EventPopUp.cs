using UnityEngine;
using TMPro;
using DG.Tweening;

public class EventPopUp : InfoPopUp
{
    public TextMeshProUGUI ButtonText;

    public void ShowEventPopUp(string Label, string Description, string Button)
    {
        Debug.Log("Started show popup");
        _controllersManager.MainGameUIController.TurnOffUI();

        LabelText.text = "";
        DescriptionText.text = "";
        ButtonText.text = "";

        transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
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

        if (_controllersManager.MainGameController.GameOverState != MainGameController.GameOverStateEnum.Playing)
        {
            _controllersManager.MainGameUIController.LoadMainMenu();
        }
    }
}