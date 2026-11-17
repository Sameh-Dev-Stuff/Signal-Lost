using Random = UnityEngine.Random;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class OffscreenEnemyWaveSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform spawnShapeCenter;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private EnemyWaveProfileSO waveProfile;

    [Header("Settings")]
    [SerializeField, Range(1.1f, 1.4f)] private float spawnDistanceMultiplier = 1.2f;

    private bool _isInsideCameraView;
    private Vector3 _viewportPosition;
    private readonly List<GameObject> _spawnedEnemies = new List<GameObject>();

    [Button]
    private void SpawnEnemy()
    {
        for (int i = 0; i < waveProfile.spawnAmount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();

            if (spawnPosition.magnitude <= Vector3.zero.magnitude)
            {
                Debug.LogError($"Enemy spawn position is not valid => spawnPosition {spawnPosition}");
                return;
            }

            GameObject enemy = Instantiate(waveProfile.targetPrefab, spawnPosition, Quaternion.identity);

            _spawnedEnemies.Add(enemy);
            
            enemy.GetComponent<EnemyController>().AssignTarget(player);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Get a random direction on the XZ plane.
        Vector3 randomDirection = Random.onUnitCircle;
        randomDirection = new Vector3(randomDirection.x, 0, randomDirection.y);
        randomDirection.Normalize();

        // Start at the player's position.
        Vector3 currentPosition = player.position;

        // Check if the starting position is inside the camera view.
        UpdateCameraVisibility(currentPosition);

        // Move along the random direction until the position is outside the camera view.
        float stepLength = 0;

        while (_isInsideCameraView)
        {
            UpdateCameraVisibility(currentPosition);

            // Make each step a little longer than the last one.
            stepLength += Time.deltaTime;

            currentPosition += randomDirection * stepLength;
        }

        // Scale the position away from the center point, so the spawn shape keeps the camera view shape.
        Vector3 offsetFromCenter = currentPosition - spawnShapeCenter.position;
        Vector3 spawnPosition = spawnShapeCenter.position + offsetFromCenter * spawnDistanceMultiplier;

        return spawnPosition;
    }

    private void UpdateCameraVisibility(Vector3 worldPosition)
    {
        // Convert the world position into viewport coordinates.
        _viewportPosition = playerCamera.WorldToViewportPoint(worldPosition);

        // Inside the view means: in front of the camera, and x and y between 0 and 1.
        _isInsideCameraView = _viewportPosition.z > 0
                              && _viewportPosition.x >= 0 && _viewportPosition.x <= 1
                              && _viewportPosition.y >= 0 && _viewportPosition.y <= 1;
    }

    [Button]
    private void DeleteSpawnedEnemies()
    {
        for (int i = 0; i < _spawnedEnemies.Count; i++)
        {
            DestroyImmediate(_spawnedEnemies[i]);
        }
        _spawnedEnemies.Clear();
    }
}