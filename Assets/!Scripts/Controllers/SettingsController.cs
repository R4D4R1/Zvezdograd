using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;
    [SerializeField] private TMP_Dropdown graphicsDropdown;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button resetButton;

    [SerializeField] private AudioSource musicAudioSource;
    private AudioSource _SFXAudioSource;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private int currentResolutionIndex;
    private bool isFullscreen;
    private float musicVolume;
    private float soundVolume;
    private int graphicsQualityIndex;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SoundVolumeKey = "SoundVolume";
    private const string FullscreenKey = "Fullscreen";
    private const string ResolutionKey = "Resolution";
    private const string GraphicsQualityKey = "GraphicsQuality";

    private void Start()
    {
        _SFXAudioSource = Bootstrapper.Instance.SoundController.GetComponent<AudioSource>();

        // Получение и фильтрация доступных разрешений экрана
        resolutions = Screen.resolutions;
        filteredResolutions = FilterResolutions(resolutions);
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        currentResolutionIndex = 0;
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string option = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
            options.Add(option);

            if (filteredResolutions[i].width == Screen.currentResolution.width &&
                filteredResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);

        // Загрузка сохраненных настроек
        LoadSettings();

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Настройка выпадающего меню качества графики
        graphicsDropdown.ClearOptions();
        List<string> graphicsOptions = new List<string> { "НИЗКОЕ", "ВЫСОКОЕ" };
        graphicsDropdown.AddOptions(graphicsOptions);

        // Установка текущего уровня качества графики
        graphicsQualityIndex = QualitySettings.GetQualityLevel() > 0 ? 1 : 0;

        fullscreenToggle.isOn = isFullscreen;
        musicVolumeSlider.value = musicVolume;
        SFXVolumeSlider.value = soundVolume;
        graphicsDropdown.value = graphicsQualityIndex;
        graphicsDropdown.RefreshShownValue();

        // Добавление слушателей для изменений настроек
        resolutionDropdown.onValueChanged.AddListener(_ => OnSettingsChanged());
        fullscreenToggle.onValueChanged.AddListener(_ => OnSettingsChanged());
        musicVolumeSlider.onValueChanged.AddListener(_ => OnMusicVolumeChanged());
        SFXVolumeSlider.onValueChanged.AddListener(_ => OnSoundVolumeChanged());
        graphicsDropdown.onValueChanged.AddListener(_ => OnSettingsChanged());
        applyButton.onClick.AddListener(ApplySettings);
        resetButton.onClick.AddListener(ResetSettings);

        applyButton.interactable = false;
        resetButton.interactable = false;
    }

    private List<Resolution> FilterResolutions(Resolution[] resolutions)
    {
        return resolutions
            .GroupBy(r => new { r.width, r.height })
            .Select(g => g.First())
            .OrderBy(r => r.width)
            .ThenBy(r => r.height)
            .ToList();
    }

    private void OnSettingsChanged()
    {
        applyButton.interactable = true;
        resetButton.interactable = true;
    }

    private void OnMusicVolumeChanged()
    {
        musicAudioSource.volume = musicVolumeSlider.value;
        OnSettingsChanged();
    }

    private void OnSoundVolumeChanged()
    {
        _SFXAudioSource.volume = SFXVolumeSlider.value;
        OnSettingsChanged();
    }

    public void ApplySettings()
    {
        int resolutionIndex = resolutionDropdown.value;
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, fullscreenToggle.isOn);

        Screen.fullScreen = fullscreenToggle.isOn;
        musicVolume = musicVolumeSlider.value;
        soundVolume = SFXVolumeSlider.value;
        QualitySettings.SetQualityLevel(graphicsDropdown.value);

        SaveSettings();

        currentResolutionIndex = resolutionIndex;
        isFullscreen = fullscreenToggle.isOn;
        graphicsQualityIndex = graphicsDropdown.value;

        applyButton.interactable = false;
        resetButton.interactable = false;
    }

    public void ResetSettings()
    {
        resolutionDropdown.value = currentResolutionIndex;
        fullscreenToggle.isOn = isFullscreen;
        musicVolumeSlider.value = musicVolume;
        SFXVolumeSlider.value = soundVolume;
        graphicsDropdown.value = graphicsQualityIndex;

        resolutionDropdown.RefreshShownValue();
        graphicsDropdown.RefreshShownValue();

        applyButton.interactable = false;
        resetButton.interactable = false;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(ResolutionKey, currentResolutionIndex);
        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.SetFloat(SoundVolumeKey, soundVolume);
        PlayerPrefs.SetInt(GraphicsQualityKey, graphicsQualityIndex);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        currentResolutionIndex = PlayerPrefs.GetInt(ResolutionKey, currentResolutionIndex);
        isFullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        soundVolume = PlayerPrefs.GetFloat(SoundVolumeKey, 1f);
        graphicsQualityIndex = PlayerPrefs.GetInt(GraphicsQualityKey, 1);

        if (musicAudioSource != null)
        {
            musicAudioSource.volume = musicVolume;
        }

        if (_SFXAudioSource != null)
        {
            _SFXAudioSource.volume = soundVolume;
        }
    }
}
