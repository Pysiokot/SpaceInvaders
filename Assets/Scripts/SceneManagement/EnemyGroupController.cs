using System;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using Zenject;

namespace SceneManagement
{
    public class EnemyGroupController : MonoBehaviour, IEnemySpawner
    {
        public event EnemiesSpawned EnemiesSpawned;
        public event EnemyKilled EnemyKilled;

        internal event EventHandler<EventArgs> EnemyCountReachedZero;

        private int _enemyCount = 0;
        private ICollection<EnemyController> _enemies;

        [Inject]
        private ISpawnStrategy _enemySpawner;


        void Start()
        {
            _enemies = _enemySpawner.SpawnEnemies();

            InitEvents();
        }

        private void InitEvents()
        {
            foreach (var enemy in _enemies)
            {
                enemy.EnemyKilled += OnEnemyKilled;
            }
        }

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
        }
    }
}
