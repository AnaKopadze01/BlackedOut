
using UnityEngine;

public class MonsterSounds : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public AudioSource audioSource;
    public AudioClip attackSound;

    [Header("Growling Settings")]
    public float hearingDistance = 10f;
    public float maximumVolume = 0.6f;

    private bool attacking = false;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.volume = 0f;
        }
    }

    void Update()
    {
        if (attacking || player == null || audioSource == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance > hearingDistance)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            return;
        }

        float closeness = 1f - distance / hearingDistance;

        audioSource.volume = Mathf.Lerp(
            0f,
            maximumVolume,
            closeness
        );

        if (!audioSource.isPlaying && audioSource.clip != null)
            audioSource.Play();
    }

    public void PlayAttackSound()
    {
        if (attacking || audioSource == null)
            return;

        attacking = true;

        audioSource.Stop();
        audioSource.loop = false;
        audioSource.volume = 0.9f;

        if (attackSound != null)
            audioSource.PlayOneShot(attackSound);
    }
}
