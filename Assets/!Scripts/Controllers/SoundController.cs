using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioClip hoverSound; // ���� ���������
    [SerializeField] private AudioClip selectionSound; // ���� ������
    [SerializeField] private AudioClip buttonPressSound; // ���� ������

    private AudioSource audioSource; // ����� ��������

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
