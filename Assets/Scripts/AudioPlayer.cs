using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer _instance;

    public AudioMixerGroup mixerGroup;

    public AudioClip fallCrash;
    public AudioClip playerDie;
    public AudioClip explosion;
    public AudioClip jump;

    private AudioSource _source;

    public enum SoundClip
    {
        FallCrash,
        PlayerDie,
        Explosion,
        Jump
    }

    private void Start()
    {
        _instance = this;
        _source = GetComponent<AudioSource>();

        _source.outputAudioMixerGroup = mixerGroup;
    }

    public static void PlaySoundClip(SoundClip clip, float delay = 0f)
    {
        _instance?.StartCoroutine(_instance.PlaySound(clip, delay));
    }

    public IEnumerator PlaySound(SoundClip clip, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        switch (clip)
        {
            case SoundClip.FallCrash:
                _source.PlayOneShot(fallCrash);
                break;
            case SoundClip.PlayerDie:
                _source.PlayOneShot(playerDie);
                break;
            case SoundClip.Explosion:
                _source.PlayOneShot(explosion);
                break;
            case SoundClip.Jump:
                _source.PlayOneShot(jump);
                break;
            default:
                Debug.Log($"Unknown sound clip {clip}");
                break;
        }
    }
}
