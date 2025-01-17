using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class MainGameUIController : MonoBehaviour
{
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _turnOffUIParent;
    [SerializeField] private float fadeDuration = 0.5f; // Длительность анимации

    private CanvasGroup _turnOffUICanvasGroup;
    //private bool _canToggleMenu = true; // Флаг для управления открытием меню через Escape
    private InfoPopUp _popUpToClose;

    private void Start()
    {
        _turnOffUICanvasGroup = _turnOffUIParent.GetComponent<CanvasGroup>();
        if (_turnOffUICanvasGroup == null)
        {
            _turnOffUICanvasGroup = _turnOffUIParent.AddComponent<CanvasGroup>();
        }

        //DisableEscapeMenuToggle();
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
        ControllersManager.Instance.mainGameController.ShowCity();
        TurnOnUI();
    }

    public void TurnOnUI()
    {
        _turnOffUIParent.SetActive(true);

        ControllersManager.Instance.blurController.UnBlurBackGroundSmoothly();
        _turnOffUICanvasGroup.DOFade(1f, fadeDuration).OnComplete(() => {
            _turnOffUICanvasGroup.interactable = true;
            _turnOffUICanvasGroup.blocksRaycasts = true;
        });

        ControllersManager.Instance.selectionController.enabled = true;
    }

    public void TurnOffUI()
    {
        ControllersManager.Instance.selectionController.enabled = false;

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
        //_canToggleMenu = true;
    }

    public void DisableEscapeMenuToggle()
    {
        //_canToggleMenu = false;
    }

    //private void Update()
    //{
    //     Проверяем, можно ли переключить меню с помощью Escape
    //    if (_canToggleMenu && Input.GetKeyUp(KeyCode.Escape))
    //    {
    //        if (currentState != GameUIState.IsRunning)
    //        {
    //            if (currentState == GameUIState.InGame)
    //            {
    //                TurnOnMenu();
    //            }
    //            else if (currentState == GameUIState.InMenu)
    //            {
    //                TurnOffMenu();
    //            }
    //            else if (currentState == GameUIState.InPopUp)
    //            {
    //                _popUpToClose.HidePopUp();
    //                ControllersManager.Instance.selectionController.Deselect();

    //                TurnOnUI();
    //                ControllersManager.Instance.mainGameController.ShowCity();
    //            }
    //        }
    //    }
    //}

    public void LoadMainMenu()
    {
        Bootstrapper.Instance.loadLevelController.LoadSceneAsync(Scenes.MAIN_MENU);
    }
}
