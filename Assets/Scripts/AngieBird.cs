using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AngieBird : MonoBehaviour
{
    private Rigidbody2D rb;
    private CircleCollider2D collider;

    private bool hasLaunched = false;
    private bool faceVelocity = false;

    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip[] launchClip;
    [SerializeField] private PhysicsMaterial2D physicsMaterial;

    private AudioSource audioSource;
    private bool isHit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();

        rb.isKinematic = true;
        collider.enabled = false;
    }

    private void FixedUpdate()
    {
        if (hasLaunched && faceVelocity)
        {
            transform.right = rb.velocity;
        }
    }

    public void LaunchBird(Vector2 direction, float force)
    {
        rb.gravityScale = 1f;
        rb.isKinematic = false;
        collider.enabled = true;

        rb.AddForce(direction * force, ForceMode2D.Impulse);
        SoundManager.instance.PlayRandomClip(launchClip, audioSource);

        hasLaunched = true;
        faceVelocity = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        faceVelocity = false;
        if (!isHit)
        {
            SoundManager.instance.PlayClip(hitClip, audioSource);
            isHit = true;
        }
    }

    public void LaunchZeroGravity(Vector2 direction, float force)
    {
        rb.gravityScale = 0f;
        rb.isKinematic = false;
        collider.enabled = true;

        rb.AddForce(direction * force, ForceMode2D.Impulse);
        SoundManager.instance.PlayRandomClip(launchClip, audioSource);

        hasLaunched = true;
        faceVelocity = true;
    }
    public void LaunchBouncyBird(Vector2 direction, float force)
    {
        physicsMaterial.bounciness = 2f;
        rb.gravityScale = 1f;
        rb.isKinematic = false;
        collider.enabled = true;

        rb.AddForce(direction * force, ForceMode2D.Impulse);
        SoundManager.instance.PlayRandomClip(launchClip, audioSource);

        hasLaunched = true;
        faceVelocity = true;
    }
}
