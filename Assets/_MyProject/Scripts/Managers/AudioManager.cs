using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public const string ALARM_SOUND = "Alarm";
    public const string ANGRY_CAT_SOUND = "Angry Cat";
    public const string BONUS_ACTIVATED_SOUND = "Bonus Activated";
    public const string BOSS_THEME_MUSIC = "Boss Theme";
    public const string BUBBLE_POP_SOUND = "Bubble Pop";
    public const string CAT_SCRATCH_SOUND = "Cat Scratch";
    public const string CAT_SELECT_SOUND = "Cat Select";
    public const string CHARACTER_DISCOVERY_SOUND = "Character Discovery";
    public const string CLOCK_TICKING_SOUND = "Clock Ticking";
    public const string COIN_COLLECT_SOUND = "Coin Collect";
    public const string COIN_SPENT_SOUND = "Coin Spent";
    public const string EXPLOSION_SOUND = "Explosion";
    public const string EXTRA_HEART_SOUND = "Extra Heart";
    public const string GAME_OVER_SOUND = "Game Over";
    public const string GIFT_COLLECT_SOUND = "Gift Collect";
    public const string GAMEPLAY_MUSIC = "Gameplay";
    public const string HIGH_SCORE_SOUND = "High Score";
    public const string ICE_BLOW_SOUND = "Ice Blow";
    public const string ICE_BREAK_SOUND = "Ice Break";
    public const string ICE_CREAM_COLLECT_SOUND = "Ice Cream Collect";
    public const string ICE_CREAM_GOAL_SOUND = "Ice Cream Goal";
    public const string ICE_CUBE_COLLECT_SOUND = "Ice Cube Collect";
    public const string LEVEL_COMPLETE_SOUND = "Level Complete";
    public const string LEVEL_UNLOCK_SOUND = "Level Unlock";
    public const string MAIN_THEME_MUSIC = "Main Theme song";
    public const string PUNCH_SOUND = "Punch";
    public const string VICTORY_SOUND = "Victory";
    public const string WHOOSH_SOUND = "Whoosh";
    public const string YOU_LOST_SOUND = "You Lost";


    [SerializeField] private List<AudioClip> audios;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            audioSource = GetComponent<AudioSource>();
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        PlayerData.UpdatedMusic += ToggleMusic;
    }

    private void OnDisable()
    {
        PlayerData.UpdatedMusic -= ToggleMusic;
    }

    private void ToggleMusic()
    {
        if (SceneController.IsDataCollectorScene)
        {
            return;
        }
        if (DataManager.Instance.PlayerData.PlayMusic)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Pause();
        }
    }

    public void PlayBackgroundMusic(string _key)
    {
        if (!DataManager.Instance.PlayerData.PlayMusic)
        {
            return;
        }
        AudioClip _audioClip = audios.Find(_element => _element.name == _key);
        if (_audioClip != null && _audioClip != audioSource.clip)
        {
            audioSource.clip = _audioClip;
            audioSource.Play();
        }
    }

    public void Play(string _key)
    {
        if (!DataManager.Instance.PlayerData.PlaySound)
        {
            return;
        }
        AudioClip _audioClip = audios.Find(_element => _element.name == _key);
        if (_audioClip != null)
        {
            audioSource.PlayOneShot(_audioClip);
        }
    }

    public void Stop(string _key)
    {
        AudioClip _audioClip = audios.Find(_element => _element.name == _key);
        if (_audioClip != null && audioSource.isPlaying && audioSource.clip == _audioClip)
        {
            audioSource.Stop();
        }
    }
}
