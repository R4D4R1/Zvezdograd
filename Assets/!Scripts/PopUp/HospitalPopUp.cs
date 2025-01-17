using DG.Tweening;
using TMPro;
using UnityEngine;

public class HospitalPopUp : InfoPopUp
{
    [SerializeField] protected TextMeshProUGUI _errorText;
    [SerializeField] protected TextMeshProUGUI _denyButtonText;

    private void Start()
    {
        _errorText.enabled = false;
        _isDestroyable = false;
    }

    public void ShowHospitalPopUp()
    {

    }
}
