public class BuildingDependingOnStability : SelectableBuilding
{
    
    protected int UpdateAmountOfTurnsNeededToDoSmth(int turnsToDoWorkOriginal)
    {
        var stability = ResourceViewModel.Stability.Value;
        var turnToDoWork = 0;

        if (stability > 75)
        {
            turnToDoWork = turnsToDoWorkOriginal - 1;
        }
        if (stability <= 75)
        {
            turnToDoWork = turnsToDoWorkOriginal;
        }
        if (stability <= 25)
        {
            turnToDoWork = turnsToDoWorkOriginal + 1;
        }

        return turnToDoWork;
    }
}
