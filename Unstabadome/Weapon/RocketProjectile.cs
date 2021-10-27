using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    WeaponManager weaponManager;
    Rigidbody rb;
    Collider[] objectsAffected;
    [SerializeField] GameObject visualExplosion;

    private void Awake()
    {
        weaponManager = FindObjectOfType<WeaponManager>();
        rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        Move();
    }
    private void Move()
    {
        rb.velocity = transform.forward * weaponManager.rocketSpeed;
    }
    private void Explode()
    {
        Debug.Log("BOOM");
        objectsAffected = Physics.OverlapSphere(transform.position, weaponManager.explosionRadius);
        foreach (Collider collider in objectsAffected)
        {

            if (collider.GetComponent<Rigidbody>() != null && collider.gameObject != gameObject)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();

                float distance = Vector3.Distance(transform.position, collider.gameObject.transform.position);

                //sets the explosion force depending on the distance of the object from the explosion
                float distancePercentage = distance / weaponManager.explosionRadius;
                float explosionForce = distancePercentage * weaponManager.maxExplosionForce;

                //sets the damage depending on the distance of the object
                float damage = distancePercentage * weaponManager.maxDamage;
                if (collider.GetComponent<IHealth>() != null)
                {
                    if (collider.GetComponent<PlayerController>() != null)
                    {
                        Debug.Log("DAMAGE PLAHYER BOM " + damage * weaponManager.rocketSelfDamageMultiplier);
                        collider.GetComponent<IHealth>().Damage(damage*weaponManager.rocketSelfDamageMultiplier, Vector3.zero, Vector3.zero);
                    }
                    else
                    {
                        collider.GetComponent<IHealth>().Damage(damage, Vector3.zero, Vector3.zero);
                    }

                }
                rb.AddExplosionForce(explosionForce * rb.mass, transform.position, weaponManager.explosionRadius);


                //Debug.Log(collider.gameObject + "Explosion force " + explosionForce + " Damage taken " + damage + " Distance " + distance.ToString() + " Percentage " + distancePercentage.ToString());
            }
        }

        GameObject newExplode = Instantiate(visualExplosion, transform.position, transform.rotation);
        Destroy(newExplode, 2);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
