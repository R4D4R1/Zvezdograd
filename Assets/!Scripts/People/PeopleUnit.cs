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
        Resting
    }

    [SerializeField] private UnitState currentState = UnitState.Ready;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Image _image;

    public int BusyTime { get; private set; }
    public int RestingTime { get; private set; }

    private void Awake()
    {
        EnableUnit();
    }

    public void EnableUnit()
    {
        Color whiteColor = Color.white;
        _image.DOColor(whiteColor, 0.5f);

        if (currentState == UnitState.Injured || currentState == UnitState.Resting)
        {
            BusyTime = 0;
            currentState = UnitState.Ready;
        }
    }

    public void DisableUnit()
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
        BusyTime = busyTurns;
        RestingTime = restingTurns;

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
        }

        UpdateStatusText();
    }

    public void SetBusyForTurns(int busyTurns, int restingTurns)
    {
        currentState = UnitState.Busy;
        BusyTime = busyTurns;
        RestingTime = restingTurns;

        UpdateStatusText();
    }

    public void SetInjured()
    {
        currentState = UnitState.Injured;

        UpdateStatusText();
    }

    public void UpdateUnitState()
    {
        if (currentState == UnitState.Busy)
        {
            
            if(BusyTime>1)
            {
                BusyTime--;
                UpdateStatusText();
            }
            else
            {
                UnitResting();
            }
        }
        else if (currentState == UnitState.Resting)
        {
            if (RestingTime > 1)
            {
                RestingTime--;
                UpdateStatusText();
            }
            else
            {
                currentState = UnitState.Ready;
                RestingTime = 0;

                _statusText.text = "";

                EnableUnit();
            }
            
        }
        else if (currentState == UnitState.Injured)
        {
            // Sorting with busy time
            BusyTime = 999;
            UpdateStatusText();
        }
    }

    public void UnitResting()
    {
        if (currentState == UnitState.Busy)
        {
            currentState = UnitState.Resting;
            UpdateStatusText();
        }
    }

    private void UpdateStatusText()
    {
        if (currentState == UnitState.Busy)
        {
            _statusText.text = $"«¿Õﬂ“ " + BusyTime;
        }
        else if (currentState == UnitState.Resting)
        {
            _statusText.text = $"Œ“ƒ€’ " + RestingTime;
        }
        else if (currentState == UnitState.Injured)
        {
            _statusText.text = $"–¿Õ≈Õ ";
        }
    }
}
