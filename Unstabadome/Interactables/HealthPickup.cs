using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : EntityBase
{
    [SerializeField] float healthAmount;

    InteractableManager interactableManager;
    HealthPickUpSFX hpSFX;
    private void Start()
    {
        interactableManager = FindObjectOfType<InteractableManager>();
        hpSFX = GetComponent<HealthPickUpSFX>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<IHealth>() != null && other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            IHealth health = player.GetComponent<IHealth>();
            if (health.CurrentHealth() < health.MaxHealth())
            {
                hpSFX.playerHealSFX();
                player.GetComponent<IHealth>().Damage(-healthAmount, Vector3.zero, Vector3.zero);
                interactableManager.PoolObject(false, gameObject);
                Disable();
            }
        }
    }

}


