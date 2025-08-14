using System.Collections.Generic;
using Unity.Hierarchy;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject playerRef;
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private float minSpawnDistance = 2f;
    [SerializeField] private float maxPawnDistance = 5f;

    [SerializeField] private float spawnInterval = 1f;

    private float nextSpawn = 0f;

    private void Update()
    {
        if (!spawnPrefab) return;
        if(nextSpawn < Time.time)
        {
            bool coinflip = Random.value > 0.5f;
            float spawnPointX = Random.Range(coinflip ? minSpawnDistance : -minSpawnDistance,
                                             coinflip ? maxPawnDistance : -maxPawnDistance);
            coinflip = Random.value > 0.5f;
            float spawnPointY = Random.Range(coinflip ? minSpawnDistance : -minSpawnDistance,
                                             coinflip ? maxPawnDistance : -maxPawnDistance);
            Vector3 spawnPos = transform.position;
            spawnPos.x += spawnPointX;
            spawnPos.y += spawnPointY;
            GameObject spawn = Instantiate(spawnPrefab, spawnPos, Quaternion.identity);
            spawn.transform.parent = transform;
            nextSpawn = Time.time + spawnInterval;
            if (!playerRef) return;
            if (spawn.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.SetTarget(playerRef);
            }
        }
    }
}
