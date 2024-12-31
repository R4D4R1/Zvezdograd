using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioClip hoverSound; // Звук наведения
    [SerializeField] private AudioClip selectionSound; // Звук выбора
    [SerializeField] private AudioClip buttonPressSound; // Звук выбора

    private AudioSource audioSource; // Аудио источник

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayHoverSound()
    {
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void PlaySelectionSound()
    {
        if (selectionSound != null)
        {
            audioSource.PlayOneShot(selectionSound);
        }
    }

    public void PlayButtonPressSound()
    {
        if (buttonPressSound != null)
        {
            audioSource.PlayOneShot(buttonPressSound);
        }
    }
}
