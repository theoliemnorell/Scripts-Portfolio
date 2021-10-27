using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaFloor : EntityBase
{
    [SerializeField] float lavaDamage, lavaBounceForce;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() != null)
        {
            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * lavaBounceForce*other.gameObject.GetComponent<Rigidbody>().mass, ForceMode.Impulse);
        }
        if (other.gameObject.GetComponent<IHealth>() != null)
        {
            other.gameObject.GetComponent<IHealth>().Damage(lavaDamage, Vector3.zero, Vector3.zero);
        }
    }
}
    




 

    
