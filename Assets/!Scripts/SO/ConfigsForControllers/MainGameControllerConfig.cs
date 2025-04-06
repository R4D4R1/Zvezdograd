using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "MainGameControllerConfig", menuName = "Configs/MainGameControllerConfig")]
public class MainGameControllerConfig : ScriptableObject
{
    [SerializeField, Range(0f, 3f)] private float blackoutTime = 1f;
    [SerializeField, Range(0.1f, 5f)] private float showCityDuration = 1f;
    [SerializeField] private float cameraCityShowY = 10f;
    [SerializeField] private float cameraCityHideY = 0f;
    [SerializeField] private Ease easeType = Ease.InOutSine;
    [SerializeField, Range(2, 5)] private int dayAfterLastArmyMaterialSendWin = 3;

    public float BlackoutTime => blackoutTime;
    public float ShowCityDuration => showCityDuration;
    public float CameraCityShowY => cameraCityShowY;
    public float CameraCityHideY => cameraCityHideY;
    public int DayAfterLastArmyMaterialSendWin => dayAfterLastArmyMaterialSendWin;
    public Ease EaseType => easeType;
}