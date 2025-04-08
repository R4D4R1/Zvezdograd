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

    public UnitState GetCurrentState() => currentState;

    public void SetState(UnitState state, int busyTurns, int restingTurns)
    {
        currentState = state;
        BusyTurns = busyTurns;
        RestingTurns = restingTurns;

        UpdateStatusText();
        HandleVisualState();
    }

    public void UpdateUnitState()
    {
        switch (currentState)
        {
            case UnitState.Busy:
                if (BusyTurns > 1)
                {
                    BusyTurns--;
                }
                else
                {
                    SetState(UnitState.Resting, 0, RestingTurns);
                }
                break;

            case UnitState.Resting:
                if (RestingTurns > 1)
                {
                    RestingTurns--;
                }
                else
                {
                    SetState(UnitState.Ready, 0, 0);
                }
                break;

            case UnitState.Injured:
                BusyTurns = 999; // Used for sorting only
                break;
        }

        UpdateStatusText();
    }

    private void HandleVisualState()
    {
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

    private void EnableUnit()
    {
        _image.DOColor(Color.white, 0.5f);
    }

    private void DisableUnit()
    {
        Color targetColor = currentState switch
        {
            UnitState.Busy => new Color(0.392f, 0.392f, 0.392f),
            UnitState.Injured => new Color(0.545f, 0f, 0f),
            _ => _image.color
        };

        _image.DOColor(targetColor, 0.5f);
    }

    private void UpdateStatusText()
    {
        _statusText.text = currentState switch
        {
            UnitState.Busy => $"ЗАНЯТ {BusyTurns}",
            UnitState.Resting => $"ОТДЫХ {RestingTurns}",
            UnitState.Ready => "СВОБОДЕН",
            UnitState.Injured => "РАНЕН",
            _ => string.Empty
        };
    }
}
