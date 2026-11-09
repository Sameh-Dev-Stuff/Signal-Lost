using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private EnemyWaveProfileSO waveProfile;
    
    [Header("Gizmos")]
    [SerializeField] private bool canDrawGizmos = true;
    [SerializeField] private float gizmosYOffset;
    private const int GizmosRingSegments = 64;

   private List<GameObject> _prefabList = new List<GameObject>();

    [Button] 
    private void SpawnPrefab()
    {

        StartCoroutine(Spawn());
        
        IEnumerator Spawn()
        {
            for (int i = 0; i < waveProfile.spawnAmount; i++)
            {
                yield return new WaitForSeconds(Random.Range(waveProfile.minSpawnDelay, waveProfile.maxSpawnDelay));

                Vector2 randomDir = Random.onUnitCircle;

                Vector3 randomPos = new Vector3(randomDir.x * waveProfile.xDistortion, 0, randomDir.y * waveProfile.yDistortion);

                float randomOffset = Random.Range(waveProfile.minSpawnRange, waveProfile.maxSpawnRange);
            
                Vector3 spawnPosition = randomPos  * randomOffset;

                spawnPosition += player.position;
                
                _prefabList.Add(Instantiate(waveProfile.targetPrefab, spawnPosition, Quaternion.identity));
            }
        }
    }
    
    [Button] 
    void DeletePrefab()
    {
        for (int i = 0; i < _prefabList.Count; i++)
        {
            DestroyImmediate(_prefabList[i]);
        }
        _prefabList.Clear();
    }
    
    private void OnDrawGizmos()
    {
        if (player == null || canDrawGizmos == false || waveProfile == null) return;
        
        Vector3 center = player.position + (Vector3.up * gizmosYOffset);

        Gizmos.color = Color.yellow;
        DrawRing(center, waveProfile.minSpawnRange);

        Gizmos.color = Color.red;
        DrawRing(center, waveProfile.maxSpawnRange);
    }

    // Draws a flat circle on the XZ plane (top-down view)
    private void DrawRing(Vector3 center, float radius)
    {
        float step = 2f * Mathf.PI / GizmosRingSegments;
        Vector3 previousPoint = center + new Vector3(radius *  waveProfile.xDistortion, 0f, 0f);

        for (int i = 1; i <= GizmosRingSegments; i++)
        {
            float angle = step * i;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * radius * waveProfile.xDistortion, 0f, Mathf.Sin(angle) * radius * waveProfile.yDistortion);

            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}