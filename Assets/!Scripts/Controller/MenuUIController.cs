using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private Volume _volumeProfile;
    [SerializeField] GameObject _turnOffUIObjects;
    [SerializeField] SelectionController _selectionController;

    private DepthOfField _depthOfField;

    private void Start()
    {
        _volumeProfile.profile.TryGet(out _depthOfField);
    }

    public void TurnOnMenu()
    {
        _depthOfField.focalLength.value = 20;
        _settingsMenu.SetActive(true);
        _turnOffUIObjects.SetActive(false);
        _selectionController.enabled = false;
    }

    public void TurnOffMenu()
    {
        _depthOfField.focalLength.value = 1;
        _settingsMenu.SetActive(false);
        _turnOffUIObjects.SetActive(true);
        _selectionController.enabled = true;

    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (_settingsMenu.activeSelf)
            {
                TurnOffMenu();
            }
            else
                TurnOnMenu();
        }
    }

    public void LoadMainMenu()
    {
        Bootstrapper.Instance.loadLevelController.LoadSceneAsync(Scenes.MAIN_MENU);
    }
}
