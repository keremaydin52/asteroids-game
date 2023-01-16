using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    [Header("UI")] 
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public Button startButton;
    public Button restartButton;

    [Header("Player")] public GameObject ship;
    
    [Header("Prefabs")]
    public GameObject bulletPrefab;
    public GameObject bigAsteroidPrefab;
    public GameObject mediumAsteroidPrefab;
    public GameObject smallAsteroidPrefab;
    
    [Header("Variables")]
    public int bulletPoolStartingSize = 10;
    public int bigAsteroidCount;

    public float spawnDistance = 5f;
    
    public int Score { get; set; }
    
    [SerializeField] 
    private GameState gameState = GameState.LoadOut;
    public static event Action<GameState, GameState> GameStateChanged;

    private readonly List<GameObject> _bigAsteroids = new List<GameObject>();
    private readonly List<GameObject> _mediumAsteroids = new List<GameObject>();
    private readonly List<GameObject> _smallAsteroids = new List<GameObject>();

    public GameState GameState
    {
        get
        {
            return gameState;
        }
        set
        {
            if (value != gameState)
            {
                GameState oldState = gameState;
                gameState = value;

                GameStateChanged?.Invoke(gameState, oldState);
            }
        }
    }

    new void Awake()
    {
        GameState = GameState.LoadOut;
    }
    
    private void Start()
    {
        Asteroid.bigAsteroidPool = new Pooler(bigAsteroidPrefab, 10);
        Asteroid.mediumAsteroidPool = new Pooler(mediumAsteroidPrefab, 10);
        Asteroid.smallAsteroidPool = new Pooler(smallAsteroidPrefab, 10);
        Bullet.bulletPool = new Pooler(bulletPrefab, bulletPoolStartingSize);

        Score = 0;
        UpdateScoreText(Score);
    }
    
    private void OnEnable()
    {
        GameStateChanged += OnGameStateChanged;
    }
    private void OnDisable()
    {
        GameStateChanged -= OnGameStateChanged;
    }
    
    private void OnGameStateChanged(GameState newState, GameState oldState)
    {
        print("newState: " + newState + " oldState: " + oldState);
        if (newState.Equals(GameState.LoadOut))
        {
            startButton.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(false);
            gameOverText.gameObject.SetActive(false);
        }
        
        if (newState.Equals(GameState.GamePlay))
        {
            startButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(false);
            gameOverText.gameObject.SetActive(false);
        }

        if (newState.Equals(GameState.GameOver))
        {
            OnGameOver();
        }
    }

    public void OnGameStart()
    {
        GameState = GameState.GamePlay;
        
        _bigAsteroids.Clear();
        _mediumAsteroids.Clear();
        _smallAsteroids.Clear();

        for (int i = 0; i < bigAsteroidCount; i++)
        {
            SpawnAsteroid();
        }
        
        ship.transform.position = Vector3.zero;
        ship.transform.rotation = Quaternion.identity;
        ship.GetComponent<Rigidbody>().isKinematic = false;
        Score = 0;
        UpdateScoreText(Score);
    }

    void OnGameOver()
    {
        startButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);

        foreach (var go in _bigAsteroids.ToList())
        {
            FreeAsteroid(AsteroidType.Big, go);
        }

        foreach (var go in _mediumAsteroids.ToList())
        {
            FreeAsteroid(AsteroidType.Medium, go);
        }

        foreach (var go in _smallAsteroids.ToList())
        {
            FreeAsteroid(AsteroidType.Small, go);
        }
    }

    // This method is to create big asteroids at the start of the game
    public void SpawnAsteroid()
    {
        Vector2 spawnDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPoint = spawnDirection * spawnDistance;
                    
        spawnPoint += transform.position;

        GameObject asteroid = Asteroid.bigAsteroidPool.Get(spawnPoint, Quaternion.identity);
        _bigAsteroids.Add(asteroid);
    }
    
    // This method is to create small and medium asteroids at bullet hit an asteroid
    public void SpawnAsteroid(AsteroidType asteroidType, Transform parent)
    {
        GameObject newAsteroid;
        
        if (asteroidType == AsteroidType.Medium)
        {
            newAsteroid = Asteroid.mediumAsteroidPool.Get(parent.position, Quaternion.identity);
            _mediumAsteroids.Add(newAsteroid);
        }
        else if (asteroidType == AsteroidType.Small)
        {
            newAsteroid = Asteroid.smallAsteroidPool.Get(parent.position, Quaternion.identity);
            _smallAsteroids.Add(newAsteroid);
        }
    }

    // Deactivate asteroid and remove from list
    public void FreeAsteroid(AsteroidType asteroidType, GameObject go)
    {
        switch (asteroidType)
        {
            case AsteroidType.Big:
                Asteroid.bigAsteroidPool.Free(go);
                _bigAsteroids.Remove(go);
                break;
            case AsteroidType.Medium:
                Asteroid.mediumAsteroidPool.Free(go);
                _mediumAsteroids.Remove(go);
                break;
            case AsteroidType.Small:
                Asteroid.smallAsteroidPool.Free(go);
                _smallAsteroids.Remove(go);
                break;
        }
    }

    // Player win the game if all asteroids are destroyed
    public bool IsWonGame()
    {
        bool wonGame = _bigAsteroids.Count == 0 && _mediumAsteroids.Count == 0 && _smallAsteroids.Count == 0;
        return wonGame;
    }

    public void UpdateScore(int amount)
    {
        Score += amount;
        UpdateScoreText(Score);
    }

    void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + Score;
    }

    public void RestartGame()
    {
        OnGameStart();
    }
}

public enum GameState
{
    LoadOut = 0,
    GamePlay = 1,
    GameOver = 2
}
