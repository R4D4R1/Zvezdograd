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
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _restingTimeText;
    [SerializeField] private Image _image;

    private int restingTime = 6;

    private void Awake()
    {
        _restingTimeText.gameObject.SetActive(false);
        EnableUnit();
    }

    public void EnableUnit()
    {
        Color whiteColor = Color.white;
        _text.DOColor(whiteColor, 0.5f);
        _image.DOColor(whiteColor, 0.5f);

        if (currentState == UnitState.Injured || currentState == UnitState.Resting)
        {
            currentState = UnitState.Ready;
        }
    }

    public void DisableUnit()
    {
        Color grayColor = new Color(0.392f, 0.392f, 0.392f);
        _text.DOColor(grayColor, 0.5f);
        _image.DOColor(grayColor, 0.5f);
    }

    public UnitState GetCurrentState()
    {
        return currentState;
    }

    public void SetBusy()
    {
        currentState = UnitState.Busy;
    }

    public void SetInjured()
    {
        currentState = UnitState.Injured;
    }

    public void SetReady()
    {
        currentState = UnitState.Ready;
    }

    public void UnitResting()
    {
        if (currentState == UnitState.Busy)
        {
            currentState = UnitState.Resting;

            restingTime = PeopleUnitsController.Instance.GetMaxRestingTime();

            UpdateRestingText();
            _restingTimeText.gameObject.SetActive(true);
        }
    }

    public void UpdateRestingTime()
    {
        if (currentState == UnitState.Resting)
        {
            restingTime--;

            UpdateRestingText();

            if (restingTime <= 0)
            {
                currentState = UnitState.Ready;
                restingTime = PeopleUnitsController.Instance.GetMaxRestingTime();
                _restingTimeText.gameObject.SetActive(false);
                EnableUnit();
            }
        }
    }

    private void UpdateRestingText()
    {
        _restingTimeText.text = restingTime.ToString();
    }
}