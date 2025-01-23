using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piggie : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    private float currentHealth;
    [SerializeField] private float damageThreshold = 0.2f;
    [SerializeField] private GameObject piggieDeathParticle;

    [SerializeField] private AudioClip deathClip;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void DamagePiggie(float damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth <= 0f)
        {
            DiePiggie();
        }
    }

    private void DiePiggie()
    {
        GameManager.instance.RemovePiggie(this);
        Instantiate(piggieDeathParticle, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(deathClip, transform.position);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude;

        if( impactVelocity > damageThreshold)
        {
            DamagePiggie(impactVelocity);
        }
    }
}
