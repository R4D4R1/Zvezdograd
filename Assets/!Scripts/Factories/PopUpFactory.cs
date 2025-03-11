using TMPro;
using UnityEngine;
using Zenject;

public class PopUpFactory
{
    private readonly Transform _parentPopUp;
    private readonly Transform _parentNotificationsPrefab;
    private readonly GameObject _infoPopUpPrefab;
    private readonly GameObject _specialPopUpPrefab;
    private readonly GameObject _notificationPrefab;
    private readonly DiContainer _container;

    public PopUpFactory(DiContainer container, Transform parentPopUp, Transform parentNotificationsPrefab, GameObject infoPopUpPrefab, GameObject specialPopUpPrefab, GameObject notificationPrefab)
    {
        _infoPopUpPrefab = infoPopUpPrefab;
        _specialPopUpPrefab = specialPopUpPrefab;
        _notificationPrefab = notificationPrefab;
        _parentPopUp = parentPopUp;
        _parentNotificationsPrefab = parentNotificationsPrefab;
        _container = container;
    }

    public GameObject CreateInfoPopUp()
    {
        return _container.InstantiatePrefab(_infoPopUpPrefab, _parentPopUp);
    }

    public GameObject CreateSpecialPopUp()
    {
        return _container.InstantiatePrefab(_specialPopUpPrefab, _parentPopUp);
    }

    public void CreateNotification(string message, bool isIncrease)
    {
        GameObject notification = _container.InstantiatePrefab(_notificationPrefab, _parentNotificationsPrefab);
        TextMeshProUGUI notificationText = notification.GetComponentInChildren<TextMeshProUGUI>();

        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.color = isIncrease ? Color.green : Color.red;
        }
    }
}
