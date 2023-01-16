using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInput : MonoBehaviour
{
    public new Rigidbody rigidbody;
    public Transform bulletSpawnPosition;

    public float thrustSpeed = 1f;
    public bool thrusting { get; private set; }

    public float turnDirection { get; private set; } = 0f;
    public float rotationSpeed = 0.1f;

    private void Update()
    {
        if(GameManager.Instance.GameState != GameState.GamePlay) return;
        
        thrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            turnDirection = 1f;
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            turnDirection = -1f;
        } else {
            turnDirection = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        if (thrusting) {
            rigidbody.velocity = transform.up * thrustSpeed;
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
        }

        if (turnDirection != 0f) {
            rigidbody.angularVelocity = rotationSpeed * turnDirection * transform.forward;
        }
        else
        {
            rigidbody.angularVelocity = Vector3.zero;
        }
        
        LimitMovement();
    }
    
    /// <summary>
    /// Limit ship movement so it cannot go out of screen bounds
    /// </summary>
    void LimitMovement()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0.05f, 0.95f);
        pos.y = Mathf.Clamp(pos.y, 0.05f, 0.95f);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    private void Shoot()
    {
        GameObject bullet = Bullet.bulletPool.Get(bulletSpawnPosition.position, transform.rotation);
        bullet.GetComponent<BulletMovement>().OnInstantiate();
    }
}
