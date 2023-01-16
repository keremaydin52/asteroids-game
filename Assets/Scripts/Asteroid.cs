using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    public AsteroidType asteroidType;

    public static Pooler bigAsteroidPool;
    public static Pooler mediumAsteroidPool;
    public static Pooler smallAsteroidPool;
    
    public float movementSpeed = 50f;

    private Rigidbody _rigidbody;


    private void OnEnable()
    {
        transform.eulerAngles = new Vector3(0f, 0f, Random.value * 360f);
        _rigidbody = GetComponent<Rigidbody>();
        MovementSpeed();
    }

    private void Update()
    {
        LimitMovement();
    }

    public void MovementSpeed()
    {
        _rigidbody.velocity = transform.up * movementSpeed;
    }
    
    /// <summary>
    /// Limit ship movement so it cannot go out of screen bounds
    /// </summary>
    void LimitMovement()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0f, 1f);
        pos.y = Mathf.Clamp(pos.y, 0f, 1f);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
}

public enum AsteroidType
{
    Big = 0,
    Medium = 1,
    Small = 2
}