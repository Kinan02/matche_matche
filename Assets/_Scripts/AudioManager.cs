using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Effects")]
    [SerializeField] private AudioClip correctChoice;
    [SerializeField] private AudioClip wrongChoice;
    [SerializeField] private AudioClip cardFlip;
    [SerializeField] private AudioClip win;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayCorrectChoice()
    {
        PlaySound(correctChoice);
    }

    public void PlayWrongChoice()
    {
        PlaySound(wrongChoice);
    }

    public void PlayCardFlip()
    {
        PlaySound(cardFlip);
    }

    public void PlayWin()
    {
        PlaySound(win);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}