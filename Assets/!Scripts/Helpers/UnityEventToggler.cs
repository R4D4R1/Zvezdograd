using UnityEngine;
using UnityEngine.Events;

public class UnityEventToggler : MonoBehaviour
{
    [SerializeField] private UnityEvent OnToggleActivate;
    [SerializeField] private UnityEvent OnToggleDeactivate;

    [SerializeField] private bool _isActivated;

    public void Toggle()
    {
        if (_isActivated)
        {
            _isActivated = false;
            OnToggleDeactivate.Invoke();
        }
        else
        {
            _isActivated = true;
            OnToggleActivate.Invoke();
        }
    }
}
