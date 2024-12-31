using UnityEngine;

public class MainGameUIController : MonoBehaviour
{
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _turnOffUIObjects;
    [SerializeField] private SelectionController _selectionController;

    private bool _canToggleMenu = false; // Флаг для управления открытием меню через Escape

    private void Start()
    {
        DisableEscapeMenuToggle();
    }

    public void TurnOnMenu()
    {
        _settingsMenu.SetActive(true);
        TurnOffUI();
        ControllersManager.Instance.mainGameController.HideCity();
    }

    public void TurnOffMenu()
    {
        _settingsMenu.SetActive(false);
        TurnOnUI();
        ControllersManager.Instance.mainGameController.ShowCity();
    }

    public void TurnOnUI()
    {
        _turnOffUIObjects.SetActive(true);
        _selectionController.enabled = true;

        ControllersManager.Instance.blurController.UnBlurBackGroundSmoothly();
    }

    public void TurnOffUI()
    {
        _turnOffUIObjects.SetActive(false);
        _selectionController.enabled = false;

        ControllersManager.Instance.blurController.BlurBackGroundSmoothly();
        ControllersManager.Instance.selectionController.Deselect();
    }

    public void EnableEscapeMenuToggle()
    {
        _canToggleMenu = true;
    }

    public void DisableEscapeMenuToggle()
    {
        _canToggleMenu = false;
    }

    private void Update()
    {
        // Проверяем, можно ли переключить меню с помощью Escape
        if (_canToggleMenu && Input.GetKeyUp(KeyCode.Escape))
        {
            if (_settingsMenu.activeSelf)
            {
                TurnOffMenu();
            }
            else
            {
                TurnOnMenu();
            }
        }
    }

    public void LoadMainMenu()
    {
        Bootstrapper.Instance.loadLevelController.LoadSceneAsync(Scenes.MAIN_MENU);
    }
}
