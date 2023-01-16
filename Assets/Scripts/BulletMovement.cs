using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public new Rigidbody rigidbody;
    public int speed;

    public void OnInstantiate()
    {
        rigidbody.velocity = transform.up * speed;
    }

    // Deactivate bullet if it's out of the screen bounds
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundary"))
        {
            Bullet.bulletPool.Free(gameObject);
        }
    }
}
