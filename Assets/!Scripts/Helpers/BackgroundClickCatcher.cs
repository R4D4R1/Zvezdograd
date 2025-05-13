using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;

public class BackgroundClickCatcher : MonoBehaviour, IPointerClickHandler
{
    public readonly Subject<Unit> OnBackgroundClicked = new();

    public void OnPointerClick(PointerEventData eventData)
    {
        OnBackgroundClicked.OnNext(Unit.Default);
    }
}
