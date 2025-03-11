using UnityEngine;

[CreateAssetMenu(fileName = "PeopleUnitsConfig", menuName = "Configs/PeopleUnitsConfig")]
public class PeopleUnitsConfig : ScriptableObject
{
    [Range(1f, 18f), SerializeField]
    private int _startPeopleUnitAmount;

    [Range(0f, 1f), SerializeField]
    private float _durationOfAnimationOfTransitionOfUnits;

    public int StartPeopleUnitAmount => _startPeopleUnitAmount;
    public float DurationOfAnimationOfTransitionOfUnits => _durationOfAnimationOfTransitionOfUnits;
}
