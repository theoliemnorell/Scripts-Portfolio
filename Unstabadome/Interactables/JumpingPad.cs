using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class JumpingPad : EntityBase
{
    [SerializeField] float jumpForce;
    [SerializeField] [EventRef] string jumpSFX;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() != null)
        {
            RuntimeManager.PlayOneShot(jumpSFX, transform.position);
            other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(other.gameObject.GetComponent<Rigidbody>().velocity.x, 0, other.gameObject.GetComponent<Rigidbody>().velocity.z);
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * jumpForce*other.gameObject.GetComponent<Rigidbody>().mass, ForceMode.Impulse);
        }
    }
    public override void Pause()
    {
        
    }

    public override void Play()
    {
        
    }

   
}
