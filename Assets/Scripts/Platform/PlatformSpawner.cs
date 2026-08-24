

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

        if (player == null) return;

        _nextSpawnX = player.position.x;

        SpawnStartingPlatform();

    }

    private void Update()

    {

        EnsureReferences();

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

            float gap    = Random.Range(difficulty.EvaluateGapMin(t), difficulty.EvaluateGapMax(t));

            float width  = difficulty.EvaluateWidth(t);

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

            if (movingEnemyPrefab != null && Random.value < movingEnemySpawnChance)

            {

                Instantiate(movingEnemyPrefab, position + Vector2.up * 0.5f, Quaternion.identity, go.transform);

            }

            else if (spikePrefab != null && Random.value < spikeSpawnChance)

            {

                Instantiate(spikePrefab, position + Vector2.up * 0.5f, Quaternion.identity, go.transform);

            }

            else if (Random.value < powerUpSpawnChance)

            {

                float r = Random.value;

                GameObject powerUp = r > 0.66f ? shieldPrefab : (r > 0.33f ? magnetPrefab : scoreMultiplierPrefab);

                if (powerUp != null)

                {

                    Instantiate(powerUp, position + Vector2.up * 0.75f, Quaternion.identity, go.transform);

                }

            }

            else if (coinPrefab != null && Random.value < coinSpawnChance)

            {

                Instantiate(coinPrefab, position + Vector2.up * 0.75f, Quaternion.identity, go.transform);

            }

        }

        else if (coinPrefab != null && Random.value < coinSpawnChance && _activePlatforms.Count > 1)

        {

            Instantiate(coinPrefab, position + Vector2.up * 0.75f, Quaternion.identity, go.transform);

        }

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

            PlayerController foundPlayer = FindFirstObjectByType<PlayerController>();

            if (foundPlayer != null) player = foundPlayer.transform;

        }

        if (PlatformPool.Instance == null)

        {

            GameObject pool = new GameObject("PlatformPool");

            pool.AddComponent<PlatformPool>();

        }

    }

    public float DifficultyT => _difficultyT;

}

