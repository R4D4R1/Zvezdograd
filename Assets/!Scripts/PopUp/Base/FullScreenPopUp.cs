
using UniRx;

public class FullScreenPopUp : InfoPopUp
{
    public override void ShowPopUp()
    {
        base.ShowPopUp();
        TimeController.DisableNextTurnLogic();
    }

    public override void HidePopUp()
    {
        base.HidePopUp();
        TimeController.EnableNextTurnLogic();
    }
}
