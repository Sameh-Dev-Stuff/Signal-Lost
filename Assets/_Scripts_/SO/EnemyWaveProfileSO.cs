using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveProfile", menuName = "Scriptable Objects/EnemyWaveProfile")]
public class EnemyWaveProfileSO : ScriptableObject
{
    [Header("References")]
    public GameObject targetPrefab;
    [Header("Settings")]
    public int   spawnAmount = 10;
    [Space]
    public float maxSpawnDelay = 0.35f;
    public float minSpawnDelay = 0.1f;
    [Space]
    public float maxSpawnRange = 18f;
    public float minSpawnRange = 16.5f;
    [Space]
    public float xDistortion = 1.67f;
    public float yDistortion = 0.9f;
    
}
