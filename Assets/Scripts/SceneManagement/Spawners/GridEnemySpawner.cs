using System.Collections.Generic;
using System.Linq;
using Enemy;
using ScriptableObjects;
using UnityEngine;

namespace SceneManagement.Spawners
{
    public class GridEnemySpawner : MonoBehaviour, ISpawnStrategy
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

        public event EnemiesSpawned EnemiesSpawned;

        internal event EnemyGroupBorderColumnsPosChanged EnemyGroupBorderColumnsPosChanged;

        [SerializeField] private GameObject _emptyGo;
        [SerializeField] private EnemiesSpawnerConfig _config;
        [SerializeField] private Vector2 _spawnBoundariesY = new Vector2(0.8f, -0.2f);
        [SerializeField] private Vector2 _spawnBoundariesX = new Vector2(0.9f, -0.9f);

        private Dictionary<int, EnemyColumn<EnemyController>> _enemies = new Dictionary<int, EnemyColumn<EnemyController>>();

        public ICollection<EnemyController> SpawnEnemies()
        {
            var result = new List<EnemyController>();

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
                    var spawnedEnemy = Instantiate(_config.EnemyGroups[i].EnemyPrefab, _enemies[colId].ColumnTransform, false);

                    spawnedEnemy.transform.localPosition = spawnPos;

                    var ec = spawnedEnemy.GetComponent<EnemyController>();
                    ec.InitParams(_config.EnemyGroups[i].EnemyParams);

                    ec.EnemyKilled += OnEnemyKilled;

                    result.Add(ec);
                    _enemies[colId].AddNewEnemy(ec);
                }
            }

            return result;
        }

        private void OnEnemyKilled(EnemyController enemyController, EnemyKilledEventArgs args)
        {
            var dictKVP = _enemies.First(kvp => kvp.Value.ColumnTransform == enemyController.transform.parent);

            _enemies[dictKVP.Key].RemoveEnemy(enemyController);
            
            if(_enemies[dictKVP.Key].Enemies.Count == 0)
            {
                HandleEnemyColumnKilled(dictKVP.Key);
            }
            else
            {
                _enemies[dictKVP.Key].GetLastEnemy().AllowShooting(5f);
            }
        }

        private int GetClosestColId(float enemyCalcPos)
        {
            int minId = 0;

            var minDst = Mathf.Abs(_enemies[0].ColumnTransform.localPosition.x - enemyCalcPos);
            for (minId = 1; minId < _enemies.Keys.Count; minId++)
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
            _enemies.Clear();

            var enemyDistance = (_spawnBoundariesX.x - _spawnBoundariesX.y) / (maxEnemyGroupCount - 1);

            for (int i = 0; i < maxEnemyGroupCount; i++)
            {
                var newGo = Instantiate(_emptyGo, this.transform, false);

                newGo.name = "Column" + (i + 1);

                var spawnPos = Vector3.right * (_spawnBoundariesX.y + (enemyDistance * i));
                newGo.transform.localPosition = spawnPos;

                _enemies.Add(i, new EnemyColumn<EnemyController>(newGo.transform, new List<EnemyController>()));
            }
        }

        private void HandleEnemyColumnKilled(int colId)
        {
            // Remove empty column reference
            _enemies.Remove(colId);

            if(_enemies.Count != 0)
            {
                CalculateNewGroupWidth();
            }
        }

        private void CalculateNewGroupWidth()
        {
            EnemyGroupBorderColumnsPosChanged?.Invoke(new Vector2(_enemies[_enemies.Keys.First()].ColumnTransform.localPosition.x, _enemies[_enemies.Keys.Last()].ColumnTransform.localPosition.x));
        }
    }
}
