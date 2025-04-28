using UnityEngine;
using TMPro;
using DG.Tweening;
using UniRx;

public class EventPopUp : InfoPopUp
{
    public TextMeshProUGUI ButtonText;

    public readonly Subject<Unit> OnEventPopUpHide = new();
    
    public void ShowEventPopUp(string label, string description, string button)
    {
        _mainGameUIController.TurnOffUI();

        LabelText.text = "";
        DescriptionText.text = "";
        ButtonText.text = "";

        transform.DOScale(Vector3.one, SCALE_DURATION).OnComplete(() =>
        {
            LabelText.text = label;
            DescriptionText.text = description;
            ButtonText.text = button;

            IsActive = true;

            SetAlpha(1);
        });
    }

    public override void HidePopUp()
    {
        base.HidePopUp();
        
        OnEventPopUpHide.OnNext(Unit.Default);
        
        if (_mainGameController.GameOverState != MainGameController.GameOverStateEnum.Playing)
        {
            _mainGameUIController.LoadMainMenu();
        }
    }
}