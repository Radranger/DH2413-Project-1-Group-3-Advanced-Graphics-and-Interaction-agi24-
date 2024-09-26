using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audioClip
        audioSource.clip = audioClip;

        //assign Volume
        audioSource.volume = volume;

        //play Sound
        audioSource.Play();

        //get Length of sound FX clip
        float clipLength = audioSource.clip.length;

        //destroy FX clip after its done playing
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        //assign a random index
        int rand = Random.Range(0, audioClip.Length);

        //spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audioClip
        audioSource.clip = audioClip[rand];

        //assign Volume
        audioSource.volume = volume;

        //play Sound
        audioSource.Play();

        //get Length of sound FX clip
        float clipLength = audioSource.clip.length;

        //destroy FX clip after its done playing
        Destroy(audioSource.gameObject, clipLength);
    }
}
