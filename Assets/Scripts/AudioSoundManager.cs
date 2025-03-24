using System.Collections;
using UnityEngine;
using GDX;

public class AmbientSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // Audio Source to play sounds
    [SerializeField] private AudioClip[] ambientSounds; // Assign 2 clips in the Inspector

    [SerializeField] private float minDelay = 5f; // Minimum delay before playing next sound
    [SerializeField] private float maxDelay = 15f; // Maximum delay before playing next sound

    private void Start()
    {
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(PlayAmbientSounds());
    }

    private IEnumerator PlayAmbientSounds()
    {
        while (true) // Infinite loop for continuous play
        {
            if (ambientSounds.Length > 0)
            {
                AudioClip randomClip = ambientSounds[Random.Range(0, ambientSounds.Length)];
                audioSource.PlayOneShot(randomClip); // Play the sound once
            }

            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime); // Wait before playing next sound
        }
    }
}
