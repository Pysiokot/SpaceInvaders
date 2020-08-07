using System;
using Enemy;
using NaughtyAttributes;
using ScriptableObjects;
using UnityEngine;

namespace SceneManagement
{
    public class EnemiesSpawnerController : MonoBehaviour
    {
        internal event EventHandler<EnemyKilledEventArgs> EnemyKilled; 
        
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private EnemiesSpawnerConfig _config;
    
        [SerializeField] private Vector2 _spawnBoundariesY = new Vector2(0.8f, -0.2f);
        [SerializeField] private Vector2 _spawnBoundariesX = new Vector2(0.9f, -0.9f);
        
        // Start is called before the first frame update
        void Start()
        {
            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            var groupsCount = _config.EnemyGroups.Count;
            var groupsDistance = (_spawnBoundariesY.x - _spawnBoundariesY.y) / (groupsCount - 1);

            var enemyCount = 0;

            for (int i = 0; i < groupsCount; i++)
            {
                enemyCount = _config.EnemyGroups[i].EnemyCount;
                var enemyDistance = (_spawnBoundariesX.x - _spawnBoundariesX.y) / (enemyCount - 1);

                for (int j = 0; j < enemyCount; j++)
                {
                    var spawnPos = new Vector3(_spawnBoundariesX.y + (enemyDistance * j), 0f, _spawnBoundariesY.y + (groupsDistance * i));
                    var spawnedEnemy = Instantiate(_enemyPrefab, this.transform, false);

                    spawnedEnemy.transform.localPosition = spawnPos;
                    
                    var ec = spawnedEnemy.GetComponent<EnemyController>();
                    ec.InitParams(_config.EnemyGroups[i].EnemyParams);

                    // Pass event forward
                    ec.EnemyKilled += (sender, args) => EnemyKilled?.Invoke(sender, args);
                }
            }
        }
        
        #if UNITY_EDITOR
        [Button("Spawn enemy prefabs")]
        public void SpawnEnemyPrefabs()
        {
            var groupsCount = _config.EnemyGroups.Count;
            var groupsDistance = (_spawnBoundariesY.x - _spawnBoundariesY.y) / (groupsCount - 1);

            var enemyCount = 0;

            for (int i = 0; i < groupsCount; i++)
            {
                enemyCount = _config.EnemyGroups[i].EnemyCount;
                var enemyDistance = (_spawnBoundariesX.x - _spawnBoundariesX.y) / (enemyCount - 1);

                for (int j = 0; j < enemyCount; j++)
                {
                    var spawnPos = new Vector3(_spawnBoundariesX.y + (enemyDistance * j), 0f, _spawnBoundariesY.y + (groupsDistance * i));
                    var spawnedEnemy = Instantiate(_enemyPrefab, this.transform, false);

                    spawnedEnemy.transform.localPosition = spawnPos;
                    
                    var ec = spawnedEnemy.GetComponent<EnemyController>();
                    ec.InitParams(_config.EnemyGroups[i].EnemyParams);
                }
            }
        }

        [Button("Clear prefabs")]
        public void ClearEnemyPrefabs()
        {
            for (int i = this.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(this.transform.GetChild(i).gameObject);
            }
        }
        #endif
    }
}
