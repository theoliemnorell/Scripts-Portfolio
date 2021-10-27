using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : EntityBase, IHealth
{

    [SerializeField] float maxExplosionForce, explosionRadius, maxDamage;
    float damage, distancePercentage;
    Collider[] objectsAffected;
    bool exploded;
    public bool explodeOnStart;
    [SerializeField] GameObject visualExplosion;
    InteractableManager interactableManager;

    private void Start()
    {
        // Debug.Log(gameObject);

        interactableManager = FindObjectOfType<InteractableManager>();
        if (explodeOnStart) Damage(0, Vector3.zero, Vector3.zero);
    }

    private void Explode()
    {
        exploded = true;
        objectsAffected = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in objectsAffected)
        {

            if (collider.GetComponent<Rigidbody>() != null && collider.gameObject != gameObject)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();

                float distance = Vector3.Distance(transform.position, collider.gameObject.transform.position);

                //sets the explosion force depending on the distance of the object from the explosion
                distancePercentage = distance / explosionRadius;
                float explosionForce = distancePercentage * maxExplosionForce;

                rb.AddExplosionForce(explosionForce * rb.mass, transform.position, explosionRadius);
                //sets the damage depending on the distance of the object
                damage = distancePercentage * maxDamage;
                if (collider.GetComponent<IHealth>() != null)
                {
                    collider.GetComponent<IHealth>().Damage(damage, Vector3.zero, Vector3.zero);

                }


                //Debug.Log(collider.gameObject + "Explosion force " + explosionForce + " Damage taken " + damage + " Distance " + distance.ToString() + " Percentage " + distancePercentage.ToString());
            }
        }

        interactableManager.PoolObject(true, gameObject);
        GameObject newExplode = Instantiate(visualExplosion, transform.position, transform.rotation);
        Destroy(newExplode, 2);
        //Destroy(this.gameObject);
        Disable();
        //GetComponent<MeshRenderer>().enabled = false;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) Damage(0, Vector3.zero, Vector3.zero);
    }
    public void Damage(float ammount, Vector3 force, Vector3 hitPoint)
    {
        if (!exploded) Explode();
    }

    public float CurrentHealth()
    {
        return 0;
    }

    public float MaxHealth()
    {
        return 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}




