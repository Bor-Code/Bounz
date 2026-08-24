using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private DifficultyConfig difficulty;
    [Header("References")]
    [SerializeField] private Transform player;
    [Header("Spawn Settings")]
    [SerializeField] private float spawnAheadDistance = 20f;
    [SerializeField] private float despawnBehindDistance = 15f;
    [SerializeField] private float startingPlatformY = -2f;
    [Header("Collectibles & Hazards")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private GameObject movingEnemyPrefab;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject magnetPrefab;
    [SerializeField] private GameObject scoreMultiplierPrefab;
    [Range(0f, 1f)] [SerializeField] private float coinSpawnChance = 0.3f;
    [Range(0f, 1f)] [SerializeField] private float spikeSpawnChance = 0.1f;
    [Range(0f, 1f)] [SerializeField] private float movingEnemySpawnChance = 0.05f;
    [Range(0f, 1f)] [SerializeField] private float powerUpSpawnChance = 0.05f;

    private readonly List<GameObject> _activePlatforms = new();
    private float _nextSpawnX;
    private float _elapsedTime;
    private float _difficultyT;

    private void Start()
    {
        EnsureReferences();
        EnsureFallbackSpawnables();
        if (player == null) return;

        _nextSpawnX = player.position.x;
        SpawnStartingPlatform();
    }

    private void Update()
    {
        EnsureReferences();
        EnsureFallbackSpawnables();
        if (player == null || difficulty == null) return;

        _elapsedTime += Time.deltaTime;
        int ticks = Mathf.FloorToInt(_elapsedTime / Mathf.Max(0.01f, difficulty.difficultyTickInterval));
        _difficultyT = Mathf.Clamp01((float)ticks / Mathf.Max(1, difficulty.ticksToFullDifficulty));
        SpawnIfNeeded();
        DespawnOld();
    }

    private void SpawnStartingPlatform()
    {
        float startWidth = difficulty.EvaluateWidth(0f);
        SpawnPlatformAt(new Vector2(player.position.x, startingPlatformY), startWidth, PlatformType.Safe);
        _nextSpawnX = player.position.x + difficulty.EvaluateGapMin(0f);
    }

    private void SpawnIfNeeded()
    {
        while (_nextSpawnX < player.position.x + spawnAheadDistance)
        {
            float t = _difficultyT;
            float gap = Random.Range(difficulty.EvaluateGapMin(t), difficulty.EvaluateGapMax(t));
            float width = difficulty.EvaluateWidth(t);
            float yOffset = Random.Range(difficulty.EvaluateHeightMin(t), difficulty.EvaluateHeightMax(t));
            float lastY = _activePlatforms.Count > 0
                ? _activePlatforms[^1].transform.position.y
                : startingPlatformY;
            Vector2 spawnPos = new Vector2(_nextSpawnX, lastY + yOffset);
            PlatformType type = difficulty.PickType(t);
            SpawnPlatformAt(spawnPos, width, type);
            _nextSpawnX += gap + width;
        }
    }

    private void SpawnPlatformAt(Vector2 position, float width, PlatformType type)
    {
        if (PlatformPool.Instance == null) return;

        GameObject go = PlatformPool.Instance.Get(position);
        go.GetComponent<Platform>()?.Initialize(type, width);
        _activePlatforms.Add(go);

        if (type != PlatformType.Safe && type != PlatformType.Fragile && _activePlatforms.Count > 1)
        {
            if (Random.value < movingEnemySpawnChance)
            {
                SpawnChild(movingEnemyPrefab, position + Vector2.up * 0.5f, go.transform);
            }
            else if (Random.value < spikeSpawnChance)
            {
                SpawnChild(spikePrefab, position + Vector2.up * 0.5f, go.transform);
            }
            else if (Random.value < powerUpSpawnChance)
            {
                float r = Random.value;
                GameObject powerUp = r > 0.66f ? shieldPrefab : (r > 0.33f ? magnetPrefab : scoreMultiplierPrefab);
                SpawnChild(powerUp, position + Vector2.up * 0.75f, go.transform);
            }
            else if (Random.value < coinSpawnChance)
            {
                SpawnChild(coinPrefab, position + Vector2.up * 0.75f, go.transform);
            }
        }
        else if (Random.value < coinSpawnChance && _activePlatforms.Count > 1)
        {
            SpawnChild(coinPrefab, position + Vector2.up * 0.75f, go.transform);
        }
    }

    private GameObject SpawnChild(GameObject prefab, Vector2 position, Transform parent)
    {
        if (prefab == null) return null;

        GameObject spawned = Instantiate(prefab, position, Quaternion.identity, parent);
        spawned.SetActive(true);
        return spawned;
    }

    private void DespawnOld()
    {
        for (int i = _activePlatforms.Count - 1; i >= 0; i--)
        {
            if (_activePlatforms[i] == null)
            {
                _activePlatforms.RemoveAt(i);
                continue;
            }

            if (_activePlatforms[i].transform.position.x < player.position.x - despawnBehindDistance)
            {
                if (PlatformPool.Instance != null)
                {
                    PlatformPool.Instance.Return(_activePlatforms[i]);
                }
                else
                {
                    _activePlatforms[i].GetComponent<Platform>()?.Cleanup();
                    Destroy(_activePlatforms[i]);
                }

                _activePlatforms.RemoveAt(i);
            }
        }
    }

    private void EnsureReferences()
    {
        if (difficulty == null) difficulty = DifficultyConfig.CreateDefault();

        if (player == null && GameManager.Instance != null && GameManager.Instance.Player != null)
            player = GameManager.Instance.Player.transform;

        if (player == null)
        {
            PlayerController foundPlayer = FindAnyObjectByType<PlayerController>();
            if (foundPlayer != null) player = foundPlayer.transform;
        }

        if (PlatformPool.Instance == null)
        {
            GameObject pool = new GameObject("PlatformPool");
            pool.AddComponent<PlatformPool>();
        }
    }

    private void EnsureFallbackSpawnables()
    {
        if (coinPrefab == null) coinPrefab = CreateFallbackSpawnable<Coin>("RuntimeCoin", new Color(1f, 0.82f, 0.16f), new Vector2(0.35f, 0.35f));
        if (spikePrefab == null) spikePrefab = CreateFallbackSpawnable<Spike>("RuntimeSpike", new Color(0.95f, 0.12f, 0.12f), new Vector2(0.45f, 0.45f));
        if (movingEnemyPrefab == null) movingEnemyPrefab = CreateFallbackSpawnable<MovingEnemy>("RuntimeMovingEnemy", new Color(0.95f, 0.2f, 0.85f), new Vector2(0.5f, 0.5f));
        if (shieldPrefab == null) shieldPrefab = CreateFallbackSpawnable<ShieldPowerUp>("RuntimeShield", new Color(0.2f, 0.85f, 1f), new Vector2(0.45f, 0.45f));
        if (magnetPrefab == null) magnetPrefab = CreateFallbackSpawnable<MagnetPowerUp>("RuntimeMagnet", new Color(1f, 0.35f, 0.35f), new Vector2(0.45f, 0.45f));
        if (scoreMultiplierPrefab == null) scoreMultiplierPrefab = CreateFallbackSpawnable<ScoreMultiplierPowerUp>("RuntimeScoreMultiplier", new Color(0.75f, 0.35f, 1f), new Vector2(0.45f, 0.45f));
    }

    private GameObject CreateFallbackSpawnable<T>(string objectName, Color color, Vector2 colliderSize) where T : Component
    {
        GameObject go = new GameObject(objectName);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GameBootstrapper.CreateRuntimeSprite();
        sr.color = color;
        go.transform.localScale = new Vector3(colliderSize.x, colliderSize.y, 1f);

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = Vector2.one;

        go.AddComponent<T>();
        go.SetActive(false);
        return go;
    }

    public float DifficultyT => _difficultyT;
}
