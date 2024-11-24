using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;


public class MainGameController : MonoBehaviour
{
    [SerializeField] private PopUp _startPopUpInfo;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private bool _tryGame;
    [SerializeField] private Image _blackImage;
    [Range(0f, 3f)]
    [SerializeField] private float _blackoutTime;

    private void Awake()
    {
        if(_tryGame)
        {
            _startPopUpInfo.gameObject.SetActive(false);


            _blackImage.color = Color.black;

            _blackImage.DOFade(0, _blackoutTime).OnComplete(() =>
            {
                _startPopUpInfo.gameObject.SetActive(true);
                _gameUI.SetActive(false);
                BlurController.Instance.BlurBackGroundNow();
                SelectionController.Instance.enabled = false;
            });
        }
        else
        {
            _startPopUpInfo.gameObject.SetActive(false);
            _gameUI.SetActive(true);
            BlurController.Instance.UnBlurBackGroundNow();
            SelectionController.Instance.enabled = true;
        }
    }
}
