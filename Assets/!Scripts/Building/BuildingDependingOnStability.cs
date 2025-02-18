using UnityEngine;

public class BuildingDependingOnStability : SelectableBuilding
{

    // ћетод мен€ющий количество шагов необходимое дл€ выполнени€ взависимости от стабильности
    protected int UpdateAmountOfTurnsNeededToDoSMTH(int TurnsToDoWorkOriginal)
    {
        int stability = ControllersManager.Instance.resourceController.GetStability();
        int turnToDoWork = 0;

        if (stability > 75)
        {
            turnToDoWork = TurnsToDoWorkOriginal - 1;
        }
        if (stability <= 75)
        {
            turnToDoWork = TurnsToDoWorkOriginal;
        }
        if (stability <= 50)
        {
            turnToDoWork = TurnsToDoWorkOriginal + 1;
        }
        if (stability <= 25)
        {
            turnToDoWork = TurnsToDoWorkOriginal + 2;
        }

        return turnToDoWork;
    }
}
