using UnityEngine;
using Zenject;

public class BuildingDependingOnStability : SelectableBuilding
{

    // ����� �������� ���������� ����� ����������� ��� ���������� ������������ �� ������������
    protected int UpdateAmountOfTurnsNeededToDoSMTH(int TurnsToDoWorkOriginal)
    {
        int stability = _resourceViewModel.Stability.Value;
        int turnToDoWork = 0;

        if (stability > 75)
        {
            turnToDoWork = TurnsToDoWorkOriginal - 1;
        }
        if (stability <= 75)
        {
            turnToDoWork = TurnsToDoWorkOriginal;
        }
        if (stability <= 25)
        {
            turnToDoWork = TurnsToDoWorkOriginal + 1;
        }

        return turnToDoWork;
    }
}
