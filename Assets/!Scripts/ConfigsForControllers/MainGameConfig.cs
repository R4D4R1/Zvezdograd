using UnityEngine;

[CreateAssetMenu(fileName = "MainGameConfig", menuName = "Configs/MainGameConfig")]
public class MainGameConfig : ScriptableObject
{
    [Header("UI PopUps")]
    [SerializeField] private InfoPopUp _startPopUp;
    [SerializeField] private InfoPopUp _tutorialPopUp;

    [Header("Blackout Settings")]
    [Range(0f, 3f)] public float _blackoutTime;

    [Header("Animation Settings")]
    [Range(0.1f, 5f)][SerializeField] private float _animationDuration = 1f;
    [SerializeField] private MainGameController.AnimationTypeEnum _animationType;

    [Header("Camera Positions")]
    [SerializeField] private float _cameraCityShowY;
    [SerializeField] private float _cameraCityHideY;

    public InfoPopUp StartPopUp => _startPopUp;
    public InfoPopUp TutorialPopUp => _tutorialPopUp;

    public float BlackoutTime => _blackoutTime;
    public float AnimationDuration => _animationDuration;
    public MainGameController.AnimationTypeEnum AnimationType => _animationType;

    public float CameraCityShowY => _cameraCityShowY;
    public float CameraCityHideY => _cameraCityHideY;

}