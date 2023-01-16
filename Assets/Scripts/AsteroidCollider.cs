using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidCollider : MonoBehaviour
{
    public Asteroid asteroid;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ship"))
        {
            GameManager.Instance.GameState = GameState.GameOver;
            other.GetComponent<Rigidbody>().isKinematic = true;
        }
        else if (other.CompareTag("Bullet"))
        {
            // Remove bullet
            Bullet.bulletPool.Free(other.gameObject);
            
            int score = 0;
            
            // Create two asteroids if player destroying a big or medium asteroid
            switch (asteroid.asteroidType)
            {
                case AsteroidType.Big:
                    score = 10;
                    GameManager.Instance.FreeAsteroid(AsteroidType.Big,gameObject);
                    GameManager.Instance.SpawnAsteroid(AsteroidType.Medium, transform);
                    GameManager.Instance.SpawnAsteroid(AsteroidType.Medium, transform);
                    break;
                case AsteroidType.Medium:
                    score = 20;
                    GameManager.Instance.FreeAsteroid(AsteroidType.Medium,gameObject);
                    GameManager.Instance.SpawnAsteroid(AsteroidType.Small, transform);
                    GameManager.Instance.SpawnAsteroid(AsteroidType.Small, transform);
                    break;
                case AsteroidType.Small:
                    score = 40;
                    GameManager.Instance.FreeAsteroid(AsteroidType.Small,gameObject);
                    if (GameManager.Instance.IsWonGame())
                    {
                        GameManager.Instance.GameState = GameState.GameOver;
                    }
                    break;
            }
            GameManager.Instance.UpdateScore(score);
        }
        else if (other.CompareTag("Boundary"))
        {
            // Reflect the asteroid if it reached the bounds
            Vector3 currentVelocity = GetComponent<Rigidbody>().velocity;
            Vector3 newDirection = Vector3.Reflect(currentVelocity, other.transform.position.normalized);
            GetComponent<Rigidbody>().velocity = newDirection;
            
            //TODO: This part could be improved for a better gameplay
        }
    }
}
