using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX; 
public class Projectile_ : MonoBehaviour
{
    [SerializeField] private float damage, speed;
    [SerializeField] GameObject fireballImpactVFX;
    [SerializeField] FMODUnity.EventReference attackImpact;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        ProjectileMovement();
    }
    public void ProjectileMovement()
    {
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {

        FMODUnity.RuntimeManager.PlayOneShot(attackImpact, transform.position);


        Instantiate(fireballImpactVFX, transform.position, transform.rotation);
        fireballImpactVFX.GetComponent<VisualEffect>().Play();
        
       

        IHealth_ hitHealth = other.transform.GetComponentInParent<IHealth_>();
        if (hitHealth != null)
        {
            Debug.Log("Hit Damage!" + other.name);
            hitHealth.ChangeHealth(-damage);
            

        }
        Destroy(this.gameObject);
    }
}
