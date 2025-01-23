using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundPlayer : MonoBehaviour, IPointerEnterHandler
{
    public enum SoundType
    {
        Hover,
        Click
    }

    public SoundType soundType;

    private Button button;
    private SoundController soundController;

    private void Awake()
    {
        button = GetComponent<Button>();
        soundController = Bootstrapper.Instance.SoundController;

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (soundType == SoundType.Hover && soundController != null)
        {
            soundController.PlayHoverSound();
        }
    }

    private void OnButtonClick()
    {
        if (soundController != null)
        {
            switch (soundType)
            {
                case SoundType.Hover:
                    soundController.PlayHoverSound();
                    break;
                case SoundType.Click:
                    soundController.PlaySelectionSound();
                    break;
            }
        }
    }
}
