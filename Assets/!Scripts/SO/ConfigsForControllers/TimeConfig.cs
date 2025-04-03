using UnityEngine;

[CreateAssetMenu(fileName = "TimeControllerConfig", menuName = "Configs/TimeControllerConfig")]
public class TimeControllerConfig : ScriptableObject
{
    [Header("TIME AND AP SETTINGS")]
    [Range(1f, 5f), SerializeField] private float nextTurnFadeTime = 1f;
    [Range(1, 10), SerializeField] private int actionPointsMaxValue = 5;
    [Range(1, 5), SerializeField] private int actionPointsAddValueInTheNextDay = 1;

    public float NextTurnFadeTime => nextTurnFadeTime;
    public int ActionPointsMaxValue => actionPointsMaxValue;
    public int ActionPointsAddValueInTheNextDay => actionPointsAddValueInTheNextDay;
}