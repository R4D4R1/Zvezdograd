using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using Zenject;

[RequireComponent(typeof(CanvasGroup))]
public class SettingsController : MonoInit
{
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _graphicsDropdown;
    [SerializeField] private Toggle _fullscreenToggle;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _SFXVolumeSlider;
    [SerializeField] private Button _applyButton;
    [SerializeField] private AudioSource _musicAudioSource;

    private Resolution[] _resolutions;
    private List<Resolution> _filteredResolutions;
    private int _currentResolutionIndex;
    private bool _isFullscreen;
    private float _musicVolume;
    private float _soundVolume;
    private int _graphicsQualityIndex;
    private CanvasGroup _canvasGroup;

    private const string SettingsFile = "settings.json";
    private SettingsData _settingsData;

    private SoundController _soundController;

    public AudioSource SettingsMusicAudioSource => _musicAudioSource;

    [Inject]
    public void Construct(SoundController soundController)
    {
        _soundController = soundController;
        Init();
    }

    public void Activate()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void Deactivate()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public override UniTask Init()
    {
        base.Init();

        _canvasGroup = GetComponent<CanvasGroup>();
        _applyButton.interactable = false;

        if (Application.isMobilePlatform)
        {
            _resolutionDropdown.gameObject.SetActive(false);
            _fullscreenToggle.gameObject.SetActive(false);
        }
        else
        {
            InitResolutionDropdown();
        }

        InitGraphicsDropdown();

        LoadSettings();
        LoadSettingsToUI();

        AddListeners();
        return UniTask.CompletedTask;
    }

    private void InitResolutionDropdown()
    {
        _resolutions = Screen.resolutions;
        _filteredResolutions = _resolutions
            .DistinctBy(r => new { r.width, r.height })
            .OrderBy(r => r.width)
            .ThenBy(r => r.height)
            .ToList();

        _resolutionDropdown.ClearOptions();
        var resolutionOptions = _filteredResolutions
            .Select(r => $"{r.width} x {r.height}")
            .ToList();
        _resolutionDropdown.AddOptions(resolutionOptions);
    }

    private void InitGraphicsDropdown()
    {
        _graphicsDropdown.ClearOptions();
        var presets = new List<string> { "Низкое", "Среднее", "Высокое" };
        _graphicsDropdown.AddOptions(presets);
    }

    private void AddListeners()
    {
        _resolutionDropdown.onValueChanged.AddListener(_ => OnSettingsChanged());
        _fullscreenToggle.onValueChanged.AddListener(_ => OnSettingsChanged());
        _musicVolumeSlider.onValueChanged.AddListener(_ => OnSettingsChanged());
        _SFXVolumeSlider.onValueChanged.AddListener(_ => OnSettingsChanged());
        _graphicsDropdown.onValueChanged.AddListener(_ => OnSettingsChanged());
        _applyButton.onClick.AddListener(ApplySettings);
    }

    private void OnSettingsChanged()
    {
        _applyButton.interactable = true;

        _musicVolume = _musicVolumeSlider.value;
        _soundVolume = _SFXVolumeSlider.value;
        _graphicsQualityIndex = _graphicsDropdown.value;

        _currentResolutionIndex = _resolutionDropdown.value;
        _isFullscreen = _fullscreenToggle.isOn;
    }

    private void ApplySettings()
    {
        if (!Application.isMobilePlatform)
        {
            Resolution resolution = _filteredResolutions[_currentResolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, _isFullscreen);
        }

        _soundController.SFXAudioSource.volume = _soundVolume;
        _musicAudioSource.volume = _musicVolume;
        QualitySettings.SetQualityLevel(_graphicsQualityIndex, false);

        SaveSettings();

        _applyButton.interactable = false;
    }

    private void SaveSettings()
    {
        _settingsData = new SettingsData
        {
            ResolutionIndex = _currentResolutionIndex,
            Fullscreen = _isFullscreen,
            MusicVolume = _musicVolume,
            SoundVolume = _soundVolume,
            GraphicsQualityIndex = _graphicsQualityIndex
        };

        File.WriteAllText(SettingsFile, JsonUtility.ToJson(_settingsData, true));
    }

    private void LoadSettings()
    {
        if (File.Exists(SettingsFile))
        {
            _settingsData = JsonUtility.FromJson<SettingsData>(File.ReadAllText(SettingsFile));
        }
        else
        {
            _settingsData = new SettingsData();
        }

        _currentResolutionIndex = _settingsData.ResolutionIndex;
        _isFullscreen = _settingsData.Fullscreen;
        _musicVolume = _settingsData.MusicVolume;
        _soundVolume = _settingsData.SoundVolume;
        _graphicsQualityIndex = _settingsData.GraphicsQualityIndex;
    }

    private void LoadSettingsToUI()
    {
        _resolutionDropdown.value = _currentResolutionIndex;
        _fullscreenToggle.isOn = _isFullscreen;
        _musicVolumeSlider.value = _musicVolume;
        _SFXVolumeSlider.value = _soundVolume;
        _graphicsDropdown.value = _graphicsQualityIndex;

        _resolutionDropdown.RefreshShownValue();
        _graphicsDropdown.RefreshShownValue();

        _musicAudioSource.volume = _musicVolume;
        _soundController.SFXAudioSource.volume = _soundVolume;

        QualitySettings.SetQualityLevel(_graphicsQualityIndex);

        if (!Application.isMobilePlatform && _filteredResolutions.Count > _currentResolutionIndex)
        {
            Resolution resolution = _filteredResolutions[_currentResolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, _isFullscreen);
        }
    }
}

[System.Serializable]
public class SettingsData
{
    public int ResolutionIndex;
    public bool Fullscreen;
    public float MusicVolume = 1f;
    public float SoundVolume = 1f;
    public int GraphicsQualityIndex = 1;
}
