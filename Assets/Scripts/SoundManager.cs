using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 统一管理各种音效
/// </summary>
public class SoundManager : MonoBehaviour
{
    // // 单例模式实现
    private static SoundManager _instance;
    public static SoundManager Instance => _instance;
    
    [Header("UI图标")]public Image muteImage;
    public Sprite muteSprite;
    public Sprite unmuteSprite;


    [Header("音效资源")] public AudioClip placementSound;
    public AudioClip victorySound;
    public AudioClip drawSound;
    public AudioClip defeatSound;

    [Header("音效设置")] [Range(0f, 1f)] public float volume = 0.7f;
    public bool isMuted;


    private AudioSource _audioSource;

    private void Awake()
    {
        // // 单例模式初始化
        if (_instance == null)
        {
            _instance = this;
            // // 确保场景切换时不销毁该对象
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
        }
        else if (_instance != this)
        {
            // // 如果已存在实例，销毁当前对象
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSource()
    {
        // // 初始化音频源组件
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.volume = volume;
        // // 根据静音状态设置UI图标
        muteImage.sprite = isMuted ? muteSprite : unmuteSprite;
    }

    public void PlayPlacementSound()
    {
        // // 播放落子音效
        PlaySound(placementSound);
    }

    public void PlayVictorySound()
    {
        // // 播放胜利音效
        PlaySound(victorySound);
    }

    public void PlayDrawSound()
    {
        // // 播放平局音效
        PlaySound(drawSound);
    }

    public void PlayDefeatSound()
    {
        // // 播放失败音效
        PlaySound(defeatSound);
    }

    private void PlaySound(AudioClip clip)
    {
        // // 播放指定的音效
        if (isMuted || clip == null || _audioSource == null) return;

        _audioSource.clip = clip;
        _audioSource.Play();
    }
    

    public void ToggleMute()
    {
        // // 切换静音状态
        isMuted = !isMuted;
        muteImage.sprite = isMuted ? muteSprite : unmuteSprite;
        if (_audioSource != null)
        {
            _audioSource.mute = isMuted;
        }
    }

    public void SetMute(bool mute)
    {
        // // 设置静音状态
        isMuted = mute;
        if (_audioSource != null)
        {
            _audioSource.mute = isMuted;
        }
    }

    public bool IsMuted()
    {
        // // 获取当前静音状态
        return isMuted;
    }
}