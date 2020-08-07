using System;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using NaughtyAttributes;
using ScriptableObjects;
using UnityEngine;

namespace SceneManagement
{
    public class EnemiesSpawnerController : MonoBehaviour
    {
        internal event EnemyKilled EnemyKilled;
        internal event EnemyGroupBorderColumnsPosChanged EnemyGroupBorderColumnsPosChanged;
        internal event EventHandler<EventArgs> EnemyCountReachedZero; 
        
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private GameObject _emptyGo;
        [SerializeField] private EnemiesSpawnerConfig _config;
    
        [SerializeField] private Vector2 _spawnBoundariesY = new Vector2(0.8f, -0.2f);
        [SerializeField] private Vector2 _spawnBoundariesX = new Vector2(0.9f, -0.9f);

        private int _enemyCount = 0;

        [ReadOnly]
        [SerializeField]
        private List<Transform> _enemyColumns;
        
        void Start()
        {
            SpawnEnemies();
        }

        private void SpawnEnemies(bool connectEvents = true)
        {
            _enemyCount = 0;
            var groupsCount = _config.EnemyGroups.Count;
            var groupsDistance = (_spawnBoundariesY.x - _spawnBoundariesY.y) / (groupsCount - 1);

            CreateEnemyColumns(_config.EnemyGroups.Max(group => group.EnemyCount));
            CalculateNewGroupWidth();

            var enemyCount = 0;

            for (int i = 0; i < groupsCount; i++)
            {
                enemyCount = _config.EnemyGroups[i].EnemyCount;
                var enemyDistance = (_spawnBoundariesX.x - _spawnBoundariesX.y) / (enemyCount - 1);

                for (int j = 0; j < enemyCount; j++)
                {
                    var colId = GetClosestColId(_spawnBoundariesX.y + (enemyDistance * j));
                    var spawnPos = new Vector3(0f, 0f, _spawnBoundariesY.y + (groupsDistance * i));
                    var spawnedEnemy = Instantiate(_enemyPrefab, _enemyColumns[colId], false);

                    spawnedEnemy.transform.localPosition = spawnPos;
                    
                    var ec = spawnedEnemy.GetComponent<EnemyController>();
                    ec.InitParams(_config.EnemyGroups[i].EnemyParams);

                    if (connectEvents)
                    {
                        ec.EnemyKilled += OnEnemyKilled;
                    }
                    
                    _enemyCount += 1;
                }
            }
        }

        private int GetClosestColId(float enemyCalcPos)
        {
            var minDst = Mathf.Abs(_enemyColumns[0].localPosition.x - enemyCalcPos);
            int minId = 0;
            for (minId = 1; minId < _enemyColumns.Count; minId++)
            {
                var newDst = Mathf.Abs(_enemyColumns[minId].localPosition.x - enemyCalcPos);

                if (newDst < minDst)
                {
                    minDst = newDst;
                    continue;
                }
                
                break;
            }

            return --minId;
        }

        private void CreateEnemyColumns(int maxEnemyGroupCount)
        {
            _enemyColumns.Clear();
            var enemyDistance = (_spawnBoundariesX.x - _spawnBoundariesX.y) / (maxEnemyGroupCount - 1);

            for (int i = 0; i < maxEnemyGroupCount; i++)
            {
                var newGo = Instantiate(_emptyGo, this.transform, false);

                newGo.name = "Column" + (i + 1);
                
                var spawnPos = new Vector3(_spawnBoundariesX.y + (enemyDistance * i), 0f,0f);
                newGo.transform.localPosition = spawnPos;
                
                _enemyColumns.Add(newGo.transform);
            }
        }

        private void OnEnemyKilled(GameObject sender, EnemyKilledEventArgs args)
        {
            // Pass event forward
            EnemyKilled?.Invoke(sender, args);

            _enemyCount -= 1;

            // Check if all enemies are dead
            if (_enemyCount == 0)
            {
                EnemyCountReachedZero?.Invoke(this, null);
            }
            // Else if this is the last Enemy in his column  
            else if (sender.transform.parent.childCount == 1)
            {
                HandleEnemyColumnKilled(_enemyColumns.FindIndex(t => t == sender.transform.parent));
            }
        }

        private void HandleEnemyColumnKilled(int colId)
        {
            _enemyColumns.RemoveAt(colId);

            CalculateNewGroupWidth();
        }

        private void CalculateNewGroupWidth()
        {
            EnemyGroupBorderColumnsPosChanged?.Invoke(new Vector2(_enemyColumns[0].localPosition.x, _enemyColumns[_enemyColumns.Count - 1].localPosition.x));
        }

#if UNITY_EDITOR
        [Button("Spawn enemy prefabs")]
        public void SpawnEnemyPrefabs()
        {
            SpawnEnemies(false);
        }

        [Button("Clear prefabs")]
        public void ClearEnemyPrefabs()
        {
            for (int i = this.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(this.transform.GetChild(i).gameObject);
            }
            
            _enemyColumns.Clear();
        }
#endif
    }
}
