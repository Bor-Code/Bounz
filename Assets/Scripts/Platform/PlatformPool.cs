

using System.Collections.Generic;

using UnityEngine;

public class PlatformPool : MonoBehaviour

{

    public static PlatformPool Instance { get; private set; }

    [Header("Prefab")]

    [Tooltip("Platform prefab'ı — Safe, Spring, Fragile, SizeShift bileşenlerinin hepsi üzerinde bulunmalı.")]

    [SerializeField] private GameObject platformPrefab;

    [Header("Pool Boyutu")]

    [Tooltip("Sahne başlangıcında önceden oluşturulacak platform sayısı.")]

    [SerializeField] private int initialPoolSize = 20;

    private readonly Queue<GameObject> _pool = new();

    private void Awake()

    {

        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;

        Prewarm();

    }

    public GameObject Get(Vector2 position)

    {

        GameObject go = _pool.Count > 0

            ? _pool.Dequeue()

            : CreateInstance();

        go.transform.position = position;

        go.SetActive(true);

        return go;

    }

    public void Return(GameObject go)

    {

        if (go == null) return;

        go.GetComponent<Platform>()?.Cleanup();

        go.SetActive(false);

        go.transform.SetParent(transform);

        _pool.Enqueue(go);

    }

    private void Prewarm()

    {

        for (int i = 0; i < initialPoolSize; i++)

        {

            GameObject go = CreateInstance();

            go.SetActive(false);

            _pool.Enqueue(go);

        }

    }

    private GameObject CreateInstance()

    {

        GameObject go = platformPrefab != null ? Instantiate(platformPrefab, transform) : CreateFallbackPlatform();

        go.SetActive(false);

        return go;

    }

    private GameObject CreateFallbackPlatform()

    {

        GameObject go = new GameObject("GeneratedPlatform");

        go.transform.SetParent(transform);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();

        sr.sprite = GameBootstrapper.CreateRuntimeSprite();

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();

        col.size = new Vector2(4f, 0.4f);

        go.AddComponent<Platform>();

        go.AddComponent<SpringPlatform>();

        go.AddComponent<FragilePlatform>();

        go.AddComponent<SizeShiftPlatform>();

        go.AddComponent<MovingPlatform>();

        return go;

    }

}

