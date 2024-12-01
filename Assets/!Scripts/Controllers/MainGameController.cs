using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;


public class MainGameController : MonoBehaviour
{
    [SerializeField] private PopUp _startPopUpInfo;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private Image _blackImage;
    [Range(0f, 3f)]
    [SerializeField] private float _blackoutTime;

    private void Awake()
    {
        _startPopUpInfo.gameObject.SetActive(false);
        SelectionController.Instance.enabled = false;

        _blackImage.color = Color.black;

        _blackImage.DOFade(0, _blackoutTime).OnComplete(() =>
        {
            _startPopUpInfo.gameObject.SetActive(true);
            _gameUI.SetActive(false);
            BlurController.Instance.BlurBackGroundNow();

        });
    }
}
