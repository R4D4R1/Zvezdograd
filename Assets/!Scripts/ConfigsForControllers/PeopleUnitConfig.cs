using UnityEngine;

[CreateAssetMenu(fileName = "PeopleUnitsControllerConfig", menuName = "Configs/PeopleUnitsControllerConfig")]
public class PeopleUnitsControllerConfig : ScriptableObject
{
    [SerializeField, Range(1f, 18f)] private int startPeopleUnitAmount = 5;
    [SerializeField, Range(0f, 1f)] private float durationOfAnimationOfTransitionOfUnits = 0.5f;
    [SerializeField, Range(0f, 100f)] private int chanceOfInjuringRandomReadyUnit = 25;

    public int StartPeopleUnitAmount => startPeopleUnitAmount;
    public float DurationOfAnimationOfTransitionOfUnits => durationOfAnimationOfTransitionOfUnits;
    public int ChanceOfInjuringRandomReadyUnit => chanceOfInjuringRandomReadyUnit;
}