using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static IDictionary<string, AudioClip> soundLibrary;

    public static AudioSource AudioSourceMusic;
    public static AudioSource AudioSourceSounds;

    // Use this for initialization
    private void Start()
    {
        AudioSourceMusic = GetComponents<AudioSource>()[0];
        AudioSourceSounds = GetComponents<AudioSource>()[1];

        soundLibrary = new Dictionary<string, AudioClip>
        {
            {"Music", Resources.Load<AudioClip>("Sounds/Music For Snake")},
            {"SnakeEat", Resources.Load<AudioClip>("Sounds/SnakeEat")},
            {"SnakeEatVirus", Resources.Load<AudioClip>("Sounds/SnakeEatVirus")},
            {"SnakeCombo", Resources.Load<AudioClip>("Sounds/SnakeCombo")}
        };

        PlayMusic();
    }

    public static void PlayMusic()
    {
        AudioSourceMusic.loop = true;
        AudioSourceMusic.clip = soundLibrary["Music"];
        AudioSourceMusic.Play();
    }

    public static void PlaySound(string soundToPlay)
    {
        AudioSourceSounds.clip = soundLibrary[soundToPlay];
        AudioSourceSounds.Play();
    }
}