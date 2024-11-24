using UnityEngine;

public class MainGameUIController : MonoBehaviour
{
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _turnOffUIObjects;
    [SerializeField] private SelectionController _selectionController;

 
    public void TurnOnMenu()
    {
        _settingsMenu.SetActive(true);
        _turnOffUIObjects.SetActive(false);
        _selectionController.enabled = false;

        BlurController.Instance.BlurBackGroundSmoothly();
    }

    public void TurnOffMenu()
    {
        _settingsMenu.SetActive(false);
        _turnOffUIObjects.SetActive(true);
        _selectionController.enabled = true;

        BlurController.Instance.UnBlurBackGroundSmoothly();
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
