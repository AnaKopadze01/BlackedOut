
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("References")]
    public AudioSource footstepAudio;

    [Header("Settings")]
    public float movementThreshold = 0.05f;
    public float stopDelay = 0.15f;

    private Rigidbody2D rb;
    private Vector2 previousPosition;
    private float stillTimer;
    private bool hasStarted;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (footstepAudio == null)
            footstepAudio = GetComponent<AudioSource>();

        footstepAudio.playOnAwake = false;
        footstepAudio.loop = true;
    }

    void Start()
    {
        previousPosition = rb.position;
    }

    void Update()
    {
        if (footstepAudio == null || footstepAudio.clip == null)
            return;

        Vector2 currentPosition = rb.position;

        float speed = Vector2.Distance(
            currentPosition,
            previousPosition
        ) / Mathf.Max(Time.deltaTime, 0.0001f);

        previousPosition = currentPosition;

        if (speed > movementThreshold)
        {
            stillTimer = 0f;

            if (!hasStarted)
            {
                footstepAudio.Play();
                hasStarted = true;
            }
            else if (!footstepAudio.isPlaying)
            {
                footstepAudio.UnPause();
            }
        }
        else
        {
            stillTimer += Time.deltaTime;

            if (stillTimer >= stopDelay && footstepAudio.isPlaying)
                footstepAudio.Pause();
        }
    }

    void OnDisable()
    {
        if (footstepAudio != null)
            footstepAudio.Stop();
    }
}
