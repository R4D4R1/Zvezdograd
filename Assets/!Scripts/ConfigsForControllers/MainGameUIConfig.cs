using UnityEngine;

[CreateAssetMenu(fileName = "MainGameUIConfig", menuName = "Configs/MainGameUIConfig")]
public class MainGameUIConfig : ScriptableObject
{
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _turnOffUIParent;
    [SerializeField] private float _fadeDuration = 0.5f;

    public GameObject SettingsMenu => _settingsMenu;
    public GameObject TurnOffUIParent => _turnOffUIParent;
    public float FadeDuration => _fadeDuration;
}