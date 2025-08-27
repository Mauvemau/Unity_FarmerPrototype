using UnityEngine;

public class Spawner : MonoBehaviour {
    [SerializeField] private GameObject playerRef;
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField, Min(0)] private float minSpawnDistance = 2f;
    [SerializeField, Min(0.1f)] private float maxPawnDistance = 5f;

    [SerializeField, Min(0)] private float spawnInterval = 1f;

    private bool spawn = true;
    private float _nextSpawn = 0f;

    private void HandleCheatInput() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            Debug.Log( spawn ? "Stopping spawning" : "Starting spawning");
            spawn = !spawn;
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            spawnInterval -= .1f;
            if (spawnInterval < 0) spawnInterval = 0;
            Debug.Log($"Spawn interval: {spawnInterval}ms");
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            spawnInterval += .1f;
            Debug.Log($"Spawn interval: {spawnInterval}ms");
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            spawnInterval = 1f;
            Debug.Log($"Spawn interval reset!");
        } 
    }
    
    private void Update() {
        HandleCheatInput();
        if (!spawn) return;
        if (!spawnPrefab) return;
        if(_nextSpawn < Time.time) {
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
