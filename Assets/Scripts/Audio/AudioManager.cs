using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public SoundSetup SoundSetup;

    public AudioMixerGroup MusicMixer;
    public AudioMixerGroup SFXMixer;

    private AudioMixer mixer;
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource jingleSource;
    
    void Awake()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = MusicMixer;
        musicSource.loop = false;
        musicSource.clip = SoundSetup.Music;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = SFXMixer;
        sfxSource.loop = false;

        jingleSource = gameObject.AddComponent<AudioSource>();
        jingleSource.outputAudioMixerGroup = MusicMixer;
        jingleSource.loop = false;
        jingleSource.volume = 0.33f;

        mixer = SFXMixer.audioMixer;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!string.IsNullOrEmpty(SoundSetup.SnapshotName))
        {
            var snapshot = mixer.FindSnapshot(SoundSetup.SnapshotName);

            mixer.TransitionToSnapshots(
                new AudioMixerSnapshot[] { snapshot }, 
                new float[] { 1f }, 
                1
            );
        }

        musicSource.Play();
    }

    void Update()
    {
        //Gestion de la loop manuellement car chaque musique à un
        //moment de loop différent
        if(musicSource.time >= SoundSetup.Music.length - 0.02f)
        {
            musicSource.time = SoundSetup.LoopStart;
        }
    }

    public void PlaySFX(AudioClip sfx)
    {
        sfxSource.PlayOneShot(sfx);
    }

    public void PlayJingle(AudioClip jingle)
    {
        musicSource.mute = true;
        jingleSource.PlayOneShot(jingle);

        StartCoroutine(JingleCoroutine(jingle.length));
    }

    IEnumerator JingleCoroutine(float jingleLength)
    {
        yield return new WaitForSeconds(jingleLength);
        musicSource.mute = false;
    }
}
