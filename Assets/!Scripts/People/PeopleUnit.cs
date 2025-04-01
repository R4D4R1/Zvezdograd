using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PeopleUnit : MonoBehaviour
{
    public enum UnitState
    {
        Ready,
        Busy,
        Injured,
        Resting,
        NotCreated,
    }

    [SerializeField] private UnitState currentState = UnitState.Ready;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Image _image;

    public int BusyTurns { get; private set; }
    public int RestingTurns { get; private set; }

    // private void Awake()
    // {
    //     EnableUnit();
    // }

    private void EnableUnit()
    {
        Color whiteColor = Color.white;
        _image.DOColor(whiteColor, 0.5f);

        // if (currentState == UnitState.Injured || currentState == UnitState.Resting)
        // {
        //     BusyTurns = 0;
        //     currentState = UnitState.Ready;
        // }
    }

    private void DisableUnit()
    {
        if (currentState == UnitState.Busy)
        {
            Color grayColor = new Color(0.392f, 0.392f, 0.392f);
            _image.DOColor(grayColor, 0.5f);
        }
        else if (currentState == UnitState.Injured)
        {
            Color darkRedColor = new Color(0.545f, 0f, 0f);
            _image.DOColor(darkRedColor, 0.5f);
        }
    }

    public UnitState GetCurrentState()
    {
        return currentState;
    }

    public void SetState(UnitState state, int busyTurns, int restingTurns)
    {
        currentState = state;
        BusyTurns = busyTurns;
        RestingTurns = restingTurns;
        
        UpdateStatusText();

        switch (currentState)
        {
            case UnitState.Ready:
                EnableUnit();
                break;
            case UnitState.Busy:
            case UnitState.Injured:
            case UnitState.Resting:
                DisableUnit();
                break;
            case UnitState.NotCreated:
                gameObject.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void UpdateUnitState()
    {
        if (currentState == UnitState.Busy)
        {
            if(BusyTurns>1)
            {
                BusyTurns--;
            }
            else
            {
                SetState(UnitState.Resting,0,RestingTurns);
            }
        }
        else if (currentState == UnitState.Resting)
        {
            if (RestingTurns > 1)
            {
                RestingTurns--;
            }
            else
            {
                SetState(UnitState.Ready,0,0);
            }
            
        }
        else if (currentState == UnitState.Injured)
        {
            // Sorting with busy turns
            BusyTurns = 999;
        }
        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        if (currentState == UnitState.Busy)
        {
            _statusText.text = $"ЗАНЯТ {BusyTurns}";
        }
        else if (currentState == UnitState.Resting)
        {
            _statusText.text = $"ОТДЫХ {RestingTurns}";
        }
        else if (currentState == UnitState.Ready)
        {
            _statusText.text = $"СВОБОДЕН";
        }
        else if (currentState == UnitState.Injured)
        {
            _statusText.text = $"РАНЕН";
        }
    }
}
