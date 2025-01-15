using UnityEngine;
using DG.Tweening;

public class MainGameUIController : MonoBehaviour
{
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _turnOffUIParent;
    [SerializeField] private SelectionController _selectionController;
    [SerializeField] private float fadeDuration = 0.5f; // Длительность анимации
    private CanvasGroup _turnOffUICanvasGroup;

    private bool _canToggleMenu = false; // Флаг для управления открытием меню через Escape

    private void Start()
    {
        _turnOffUICanvasGroup = _turnOffUIParent.GetComponent<CanvasGroup>();
        if (_turnOffUICanvasGroup == null)
        {
            _turnOffUICanvasGroup = _turnOffUIParent.AddComponent<CanvasGroup>();
        }

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
        Debug.Log("Turn on ui");

        _turnOffUIParent.SetActive(true);
        _selectionController.enabled = true;

        ControllersManager.Instance.blurController.UnBlurBackGroundSmoothly();
        _turnOffUICanvasGroup.DOFade(1f, fadeDuration).OnComplete(() => {
            _turnOffUICanvasGroup.interactable = true;
            _turnOffUICanvasGroup.blocksRaycasts = true;
        });
    }

    public void TurnOffUI()
    {
        Debug.Log("Turn off ui");
        _selectionController.enabled = false;

        ControllersManager.Instance.blurController.BlurBackGroundSmoothly();
        ControllersManager.Instance.selectionController.Deselect();

        _turnOffUICanvasGroup.DOFade(0f, fadeDuration).OnComplete(() => {
            _turnOffUIParent.SetActive(false);
            _turnOffUICanvasGroup.interactable = false;
            _turnOffUICanvasGroup.blocksRaycasts = false;
        });
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
