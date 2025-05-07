using UnityEngine;

public class SubscribeUIController : MonoBehaviour
{
    [SerializeField] private UIController UIController;

    private void Start()
    {
        var saveLoadController = FindFirstObjectByType<SaveLoadController>();
        saveLoadController.SubscribeUIControllerInMainMenu(UIController);
    }
}
