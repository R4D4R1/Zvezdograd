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
    [SerializeField] private Image _image;

    private int restingTime = 6;

    private void Awake()
    {
        // Initialize colors
        EnableUnit(); // Start with unit enabled
    }

    public void EnableUnit()
    {
        // UI change to white color
        Color whiteColor = Color.white;
        _text.DOColor(whiteColor, 0.5f); // Adjust duration as needed
        _image.DOColor(whiteColor, 0.5f);

        if (currentState == UnitState.Injured || currentState == UnitState.Resting)
        {
            currentState = UnitState.Ready;
            Debug.Log(gameObject.name + " is now Ready.");
        }
    }

    public void DisableUnit()
    {
        // UI change to gray color
        Color grayColor = new Color(0.392f, 0.392f, 0.392f); // RGB 100/255
        _text.DOColor(grayColor, 0.5f); // Adjust duration as needed
        _image.DOColor(grayColor, 0.5f);
    }

    public UnitState GetCurrentState()
    {
        return currentState;
    }

    public void SetState(UnitState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case UnitState.Ready:
                Debug.Log(gameObject.name + " is now Ready.");
                break;
            case UnitState.Busy:
                Debug.Log(gameObject.name + " is now Busy.");
                break;
            case UnitState.Injured:
                Debug.Log(gameObject.name + " is now Injured.");
                break;
            case UnitState.Resting:
                Debug.Log(gameObject.name + " has started resting.");
                break;
        }
    }

    public void SetBusy()
    {
        currentState = UnitState.Busy;
        Debug.Log(gameObject.name + " is now Busy.");
    }

    public void SetInjured()
    {
        currentState = UnitState.Injured;
        Debug.Log(gameObject.name + " is now Injured.");
    }

    public void SetReady()
    {
        currentState = UnitState.Ready;
        Debug.Log(gameObject.name + " is now Ready.");
    }

    public void UnitResting()
    {
        if (currentState == UnitState.Busy)
        {
            currentState = UnitState.Resting;
            restingTime = UnitManager.Instance.GetMaxRestingTime();
            Debug.Log(gameObject.name + " has started resting.");
        }
    }

    public void UpdateRestingTime()
    {
        if (currentState == UnitState.Resting)
        {
            restingTime--;

            Debug.Log(gameObject.name + " Resting left = " + restingTime);

            if (restingTime <= 0)
            {
                currentState = UnitState.Ready;
                restingTime = UnitManager.Instance.GetMaxRestingTime();
                EnableUnit();
                Debug.Log(gameObject.name + " has finished resting and is now Ready.");
            }
        }
    }
}
