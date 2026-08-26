using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField, Range(15,30)] private float speed;
    [SerializeField, Range(0,4)] private float lifeTime;

    private void Start()
    {
        rb.AddForce(transform.forward * speed , ForceMode.Impulse);
        
        Destroy(gameObject, lifeTime);
    }
}
