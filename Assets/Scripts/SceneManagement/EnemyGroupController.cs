#define aUSE_LIST

using System;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using NaughtyAttributes;
using ScriptableObjects;
using UnityEngine;

namespace SceneManagement
{
    public class EnemyGroupController : MonoBehaviour
    {
        private struct EnemyColumn<T>
        {
            public Transform ColumnTransform { get; private set; }
            public ICollection<T> Enemies { get; private set; }

            public EnemyColumn(Transform t, ICollection<T> enemies)
            {
                ColumnTransform = t;
                Enemies = enemies;
            }

            public void AddNewEnemy(T enemy)
            {
                Enemies.Add(enemy);
            }

            public void RemoveEnemy(T enemy)
            {
                Enemies.Remove(enemy);
            }

            public T GetLastEnemy()
            {
                return Enemies.Last();
            }
        }

        public event EnemyKilled EnemyKilled;
        public event EnemiesSpawned EnemiesSpawned;

        internal event EnemyGroupBorderColumnsPosChanged EnemyGroupBorderColumnsPosChanged;
        internal event EventHandler<EventArgs> EnemyCountReachedZero; 
        
        [SerializeField] private GameObject _enemyPrefab;
        // Needed to separate spawned enemies to columns without creating unuseful gameobjects in scene
        [SerializeField] private GameObject _emptyGo;
        [SerializeField] private EnemiesSpawnerConfig _config;
    
        [SerializeField] private Vector2 _spawnBoundariesY = new Vector2(0.8f, -0.2f);
        [SerializeField] private Vector2 _spawnBoundariesX = new Vector2(0.9f, -0.9f);

        private int _enemyCount = 0;

        [SerializeField]
        [ReadOnly]
        private EnemyController _ec;

#if USE_LIST
        [ReadOnly]
        [SerializeField]
        private List<Transform> _enemyColumns;
#else
        private Dictionary<int, EnemyColumn<EnemyController>> _enemies = new Dictionary<int, EnemyColumn<EnemyController>>();
#endif
        
        void Start()
        {
            SpawnEnemies();
        }

#region Spawning Enemies
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
                    var spawnPos = Vector3.forward * (_spawnBoundariesY.x - (groupsDistance * i));
#if USE_LIST
                    var spawnedEnemy = Instantiate(_enemyPrefab, _enemyColumns[colId], false);
#else
                    var spawnedEnemy = Instantiate(_enemyPrefab, _enemies[colId].ColumnTransform, false);
#endif

                    spawnedEnemy.transform.localPosition = spawnPos;
                    
                    var ec = spawnedEnemy.GetComponent<EnemyController>();
                    ec.InitParams(_config.EnemyGroups[i].EnemyParams);

#if !USE_LIST
                    _enemies[colId].AddNewEnemy(ec); 
#endif
                    if (connectEvents)
                    {
                        ec.EnemyKilled += OnEnemyKilled;
                    }

                    // last enemy row
                    if(i == groupsCount - 1)
                    {
                        ec.AllowShooting(_enemyCount / 4f);
                    }
                    
                    _enemyCount += 1;
                }
            }

            EnemiesSpawned?.Invoke(_enemyCount);
        }

        private int GetClosestColId(float enemyCalcPos)
        {
            int minId = 0;

#if USE_LIST
            var minDst = Mathf.Abs(_enemyColumns[0].localPosition.x - enemyCalcPos);
            for (minId = 1; minId < _enemyColumns.Count; minId++)
#else
            var minDst = Mathf.Abs(_enemies[0].ColumnTransform.localPosition.x - enemyCalcPos);
            for (minId = 1; minId < _enemies.Keys.Count; minId++)
#endif
            {
                var newDst = Mathf.Abs(_enemies[minId].ColumnTransform.localPosition.x - enemyCalcPos);

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
#if USE_LIST
            _enemyColumns.Clear();
#else
            _enemies.Clear();
#endif

            var enemyDistance = (_spawnBoundariesX.x - _spawnBoundariesX.y) / (maxEnemyGroupCount - 1);

            for (int i = 0; i < maxEnemyGroupCount; i++)
            {
                var newGo = Instantiate(_emptyGo, this.transform, false);

                newGo.name = "Column" + (i + 1);
                
                var spawnPos = Vector3.right * (_spawnBoundariesX.y + (enemyDistance * i));
                newGo.transform.localPosition = spawnPos;

#if USE_LIST
                _enemyColumns.Add(newGo.transform);
#else
                _enemies.Add(i, new EnemyColumn<EnemyController>(newGo.transform, new List<EnemyController>()));
#endif
            }
        }
#endregion

        private void OnEnemyKilled(EnemyController sender, EnemyKilledEventArgs args)
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
            else 
            {
#if USE_LIST
                var columnEnemyCount = sender.transform.parent.childCount;

                // last enemy in this column
                if (columnEnemyCount == 1)
                {
                    HandleEnemyColumnKilled(_enemyColumns.FindIndex(t => t == sender.transform.parent));
                }
                else if(sender.transform == sender.transform.parent.GetChild(columnEnemyCount - 1)) // first of enemies in this column
                {
                    // TODO: Refactor
                    sender.transform.parent.GetChild(columnEnemyCount - 2).GetComponent<EnemyController>().AllowShooting(_enemyCount / 2f);
                }
#else
                var colId = _enemies.Where(kv => kv.Value.ColumnTransform == sender.transform.parent).First().Key;
                _enemies[colId].RemoveEnemy(sender);

                var columnEnemyCount = _enemies[colId].Enemies.Count;

                // last enemy in this column
                if (columnEnemyCount == 0)
                {
                    HandleEnemyColumnKilled(colId);
                }
                else
                {
                    _ec = _enemies[colId].GetLastEnemy();
                    _enemies[colId].GetLastEnemy().AllowShooting(_enemyCount / 4f);
                }
#endif
            }
        }

        private void HandleEnemyColumnKilled(int colId)
        {
            // Remove empty column reference
#if USE_LIST
            _enemyColumns.RemoveAt(colId);
#else
            _enemies.Remove(colId);
#endif

            CalculateNewGroupWidth();
        }

        private void CalculateNewGroupWidth()
        {
#if USE_LIST
            EnemyGroupBorderColumnsPosChanged?.Invoke(new Vector2(_enemyColumns[0].localPosition.x, _enemyColumns[_enemyColumns.Count - 1].localPosition.x));
#else
            EnemyGroupBorderColumnsPosChanged?.Invoke(new Vector2(_enemies[_enemies.Keys.First()].ColumnTransform.localPosition.x, _enemies[_enemies.Keys.Last()].ColumnTransform.localPosition.x));
#endif
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

#if USE_LIST
            _enemyColumns.Clear();
#else
            _enemies.Clear();
#endif
        }
#endif
        }
}
