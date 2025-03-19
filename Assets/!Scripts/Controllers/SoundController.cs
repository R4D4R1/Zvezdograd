using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip selectionSound;
    [SerializeField] private AudioClip buttonPressSound;

    public AudioSource SFXAudioSource;

    private void Awake()
    {
        SFXAudioSource = GetComponent<AudioSource>();
    }

    public void PlayHoverSound()
    {
        if (hoverSound != null && Application.platform != RuntimePlatform.Android)
        {
            SFXAudioSource.PlayOneShot(hoverSound);
        }
    }

    public void PlaySelectionSound()
    {
        if (selectionSound != null)
        {
            SFXAudioSource.PlayOneShot(selectionSound);
        }
    }

    public void PlayButtonPressSound()
    {
        if (buttonPressSound != null)
        {
            SFXAudioSource.PlayOneShot(buttonPressSound);
        }
    }
}
