using System;
using UnityEngine;

public class FrozenChilliController : MonoBehaviour
{
    public static FrozenChilliController Instance;
    public static Action OnUnfrozen;

    [SerializeField] private float freezeTime;
    private float freezeCounter;

    public bool freezeChillies;
    public bool playIceBreakSound;
    public bool isPlaying;
    public int iceBreakSoundCall;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        freezeChillies = false;
        freezeTime = FirebaseRemoteConfigManager.Instance.iceButtonConfigData.freezeTime;
        freezeCounter = 0;
        iceBreakSoundCall = 0;
        playIceBreakSound = false;
        isPlaying = false;
    }

    private void Update()
    {
        if (freezeChillies)
        {
            freezeCounter += Time.deltaTime;

            if (freezeCounter >= freezeTime)
            {
                freezeChillies = false;
                freezeCounter = 0;
                OnUnfrozen?.Invoke();
            }
        }

        if (iceBreakSoundCall > 0)
        {
            playIceBreakSound = true;
        }

        if (playIceBreakSound && !isPlaying)
        {
            isPlaying = true;
            PlayIceBreakSound();
        }
    }

    private void PlayIceBreakSound()
    {
        iceBreakSoundCall = 0;
        playIceBreakSound = false;

        AudioManager.Instance.Play(AudioManager.ICE_BREAK_SOUND);

        isPlaying = false;
    }
}
