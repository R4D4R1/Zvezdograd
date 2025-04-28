
using UniRx;

public class FullScreenPopUp : InfoPopUp
{
    public override void ShowPopUp()
    {
        base.ShowPopUp();
        _timeController.DisableNextTurnLogic();
    }

    public override void HidePopUp()
    {
        base.HidePopUp();
        _timeController.EnableNextTurnLogic();
    }
}
