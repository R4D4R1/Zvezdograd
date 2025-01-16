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
    [SerializeField] private TextMeshProUGUI _statusText; // ����� ��� ���������
    [SerializeField] private Image _image;

    public int BusyTime { get; private set; }    // ����� ������
    public int RestingTime { get; private set; } // ����� ������

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
        BusyTime = busyTurns;
        RestingTime = restingTurns;

        UpdateStatusText();
        _statusText.gameObject.SetActive(true);
    }

    public void UpdateUnitState()
    {
        if (currentState == UnitState.Busy)
        {
            BusyTime--;
            UpdateStatusText();

            if (BusyTime <= 0)
            {
                UnitResting(); // ������� � ��������� "������"
            }
        }
        else if (currentState == UnitState.Resting)
        {
            RestingTime--;
            UpdateStatusText();

            if (RestingTime <= 0)
            {
                currentState = UnitState.Ready;
                RestingTime = 0;

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
            _statusText.text = $"����� " + BusyTime;
        }
        else if (currentState == UnitState.Resting)
        {
            _statusText.text = $"����� " + RestingTime;
        }
    }
}
