using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudio;
    [SerializeField] private AudioSource effectAudio;

    [SerializeField] private AudioClip[] clips;

    void Start()
    {
        BGMSoundPlay("MainBGM");
    }

    public void BGMSoundPlay(string clipName)
    {
        foreach (var clip in clips)
        {
            if (clip.name == clipName)
            {
                bgmAudio.clip = clip;
                bgmAudio.Play();

                return;
            }
        }
        Debug.Log($"{clipName}을 찾지 못했습니다.");
    }

    public void EffectSoundPlay(string clipName)
    {
        foreach (var clip in clips)
        {
            if (clip.name == clipName)
            {
                effectAudio.PlayOneShot(clip);

                return;
            }
        }
        Debug.Log($"{clipName}을 찾지 못했습니다.");
    }

    public void BGMOnOff(bool isMute)
    {
        bgmAudio.mute = isMute;
    }

    public void EffectOnOff(bool isMute)
    {
        effectAudio.mute = isMute;
    }
}
