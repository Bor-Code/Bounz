using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private DifficultyConfig difficulty;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject platformPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnAheadDistance = 20f;
    [SerializeField] private float despawnBehindDistance = 15f;
    [SerializeField] private float startingPlatformY = -2f;

    private readonly List<GameObject> _activePlatforms = new();
    private float _nextSpawnX;
    private float _elapsedTime;
    private float _difficultyT;

    private void Start()
    {
        _nextSpawnX = player.position.x;
        SpawnStartingPlatform();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        int ticks = Mathf.FloorToInt(_elapsedTime / difficulty.difficultyTickInterval);
        _difficultyT = Mathf.Clamp01((float)ticks / difficulty.ticksToFullDifficulty);

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
        GameObject go = Instantiate(platformPrefab, position, Quaternion.identity);
        go.GetComponent<Platform>().Initialize(type, width);
        _activePlatforms.Add(go);
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
                Destroy(_activePlatforms[i]);
                _activePlatforms.RemoveAt(i);
            }
        }
    }

    public float DifficultyT => _difficultyT;
}
