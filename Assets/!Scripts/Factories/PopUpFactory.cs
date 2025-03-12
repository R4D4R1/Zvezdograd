using System.Collections.Generic;
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

    private readonly Queue<GameObject> _infoPopUpPool = new();
    private readonly Queue<GameObject> _specialPopUpPool = new();
    private readonly Queue<GameObject> _notificationPool = new();

    public PopUpFactory(DiContainer container, Transform parentPopUp, Transform parentNotificationsPrefab,
                        GameObject infoPopUpPrefab, GameObject specialPopUpPrefab, GameObject notificationPrefab)
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
        return GetOrCreate(_infoPopUpPool, _infoPopUpPrefab, _parentPopUp);
    }

    public GameObject CreateSpecialPopUp()
    {
        return GetOrCreate(_specialPopUpPool, _specialPopUpPrefab, _parentPopUp);
    }

    public GameObject CreateNotification(string message, bool isIncrease)
    {
        GameObject notification = GetOrCreate(_notificationPool, _notificationPrefab, _parentNotificationsPrefab);
        TextMeshProUGUI notificationText = notification.GetComponentInChildren<TextMeshProUGUI>();

        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.color = isIncrease ? Color.green : Color.red;
        }

        return notification;
    }

    private GameObject GetOrCreate(Queue<GameObject> pool, GameObject prefab, Transform parent)
    {
        if (pool.Count > 0)
        {
            GameObject pooledObject = pool.Dequeue();
            pooledObject.SetActive(true);
            return pooledObject;
        }

        return _container.InstantiatePrefab(prefab, parent);
    }

    public void ReturnInfoPopUpToPool(InfoPopUp popUp)
    {
        popUp.gameObject.SetActive(false);

        if (popUp is SpecialPopUp)
            _specialPopUpPool.Enqueue(popUp.gameObject);
        else
            _infoPopUpPool.Enqueue(popUp.gameObject);
    }

    public void ReturnNotificationToPool(Notification notification)
    {
        notification.gameObject.SetActive(false);
        _notificationPool.Enqueue(notification.gameObject);
    }
}
