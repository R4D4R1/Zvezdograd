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
    [SerializeField] private TextMeshProUGUI _statusText; // Текст для состояния
    [SerializeField] private Image _image;

    public int restingTime; // Время отдыха
    public int busyTime;    // Время работы

    private void Awake()
    {
        _statusText.gameObject.SetActive(false);
        EnableUnit();
    }

    public void EnableUnit()
    {
        Color whiteColor = Color.white;
        _image.DOColor(whiteColor, 0.5f);

        _statusText.gameObject.SetActive(false);

        if (currentState == UnitState.Injured || currentState == UnitState.Resting)
        {
            currentState = UnitState.Ready;
        }
    }

    public void DisableUnit()
    {
        Color grayColor = new Color(0.392f, 0.392f, 0.392f);
        _image.DOColor(grayColor, 0.5f);
    }

    public UnitState GetCurrentState()
    {
        return currentState;
    }

    public void SetBusyForTurns(int busyTurns, int restingTurns)
    {
        currentState = UnitState.Busy;
        busyTime = busyTurns;
        restingTime = restingTurns;

        UpdateStatusText();
        _statusText.gameObject.SetActive(true);
    }

    public void UpdateUnitState()
    {
        if (currentState == UnitState.Busy)
        {
            busyTime--;
            UpdateStatusText();

            if (busyTime <= 0)
            {
                UnitResting(); // Переход в состояние "отдыха"
            }
        }
        else if (currentState == UnitState.Resting)
        {
            restingTime--;
            UpdateStatusText();

            if (restingTime <= 0)
            {
                currentState = UnitState.Ready;
                restingTime = 0;

                _statusText.gameObject.SetActive(false);
                EnableUnit();
            }
        }
    }

    public void UnitResting()
    {
        if (currentState == UnitState.Busy)
        {
            currentState = UnitState.Resting;
            UpdateStatusText();

            _statusText.gameObject.SetActive(true);
        }
    }

    private void UpdateStatusText()
    {
        if (currentState == UnitState.Busy)
        {
            _statusText.text = $"ЗАНЯТ";
        }
        else if (currentState == UnitState.Resting)
        {
            _statusText.text = $"ОТДЫХ";
        }
    }
}
