using UnityEngine;
using DG.Tweening;
using Zenject;

public class MainGameUIController : MonoBehaviour
{
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _turnOffUIParent;
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup _turnOffUICanvasGroup;
    //private bool _canToggleMenu = true;
    private InfoPopUp _popUpToClose;

    private readonly ResourceView _resourceView;
    protected ControllersManager _controllersManager;

    [Inject]
    public MainGameUIController(ResourceView resourceView)
    {
        _resourceView = resourceView;
    }

    [Inject]
    public void Construct(ControllersManager controllersManager)
    {
        _controllersManager = controllersManager;
    }

    private void Start()
    {
        _turnOffUICanvasGroup = _turnOffUIParent.GetComponent<CanvasGroup>();
        if (_turnOffUICanvasGroup == null)
        {
            _turnOffUICanvasGroup = _turnOffUIParent.AddComponent<CanvasGroup>();
        }

        _turnOffUICanvasGroup.alpha = 0;
        _turnOffUICanvasGroup.interactable = false;
        _turnOffUICanvasGroup.blocksRaycasts = false;

        //DisableEscapeMenuToggle();
    }

    public void TurnOnMenu()
    {
        _settingsMenu.SetActive(true);
        TurnOffUI();
        _controllersManager.MainGameController.HideCity();
    }

    public void TurnOffMenu()
    {
        _settingsMenu.SetActive(false);
        _controllersManager.MainGameController.ShowCity();
        TurnOnUI();
    }

    public void TurnOnUI()
    {
        //_turnOffUIParent.SetActive(true);

        _controllersManager.BlurController.UnBlurBackGroundSmoothly();
        _turnOffUICanvasGroup.DOFade(1f, fadeDuration).OnComplete(() => {
            _turnOffUICanvasGroup.interactable = true;
            _turnOffUICanvasGroup.blocksRaycasts = true;
        });

        _controllersManager.SelectionController.enabled = true;
    }

    public void TurnOffUI()
    {
        _controllersManager.SelectionController.enabled = false;
        _controllersManager.BlurController.BlurBackGroundSmoothly();
        _controllersManager.SelectionController.Deselect();

        _turnOffUICanvasGroup.DOFade(0f, fadeDuration).OnComplete(() => {
            //_turnOffUIParent.SetActive(false);
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
        Bootstrapper.Instance?.loadLevelController.LoadSceneAsync(Scenes.MAIN_MENU);
    }
}
