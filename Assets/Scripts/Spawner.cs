using UnityEngine;

public class Spawner : MonoBehaviour {
    [SerializeField] private GameObject playerRef;
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private float minSpawnDistance = 2f;
    [SerializeField] private float maxPawnDistance = 5f;

    [SerializeField] private float spawnInterval = 1f;

    private float _nextSpawn = 0f;

    private void Update() {
        if (!spawnPrefab) return;
        if(_nextSpawn < Time.time)
        {
            bool coinFlip = Random.value > 0.5f;
            float spawnPointX = Random.Range(coinFlip ? minSpawnDistance : -minSpawnDistance,
                coinFlip ? maxPawnDistance : -maxPawnDistance);
            coinFlip = Random.value > 0.5f;
            float spawnPointY = Random.Range(coinFlip ? minSpawnDistance : -minSpawnDistance,
                coinFlip ? maxPawnDistance : -maxPawnDistance);
            Vector3 spawnPos = transform.position;
            spawnPos.x += spawnPointX;
            spawnPos.y += spawnPointY;
            GameObject spawn = Instantiate(spawnPrefab, spawnPos, Quaternion.identity);
            spawn.transform.parent = transform;
            _nextSpawn = Time.time + spawnInterval;
            if (!playerRef) return;
            if (spawn.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.SetTarget(playerRef);
            }
        }
    }
}
