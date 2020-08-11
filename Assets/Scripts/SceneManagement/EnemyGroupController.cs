using System;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using Utils;
using Zenject;

namespace SceneManagement
{
    public class EnemyGroupController : MonoBehaviour, IEnemySpawner, IEnemyLifeController, IEnemyGroupLifeController
    {
        public event EnemiesSpawned EnemiesSpawned;
        public event EnemyKilled EnemyKilled;

        public event EventHandler<EventArgs> EnemyCountReachedZero;

        private int _enemyCount = 0;
        private ICollection<EnemyController> _enemies;

        private ISpawnStrategy _spawnStrategy;
        private IGameStateController _gameStateController;

        [Inject]
        private void InitializeDI(ISpawnStrategy spawnStrategy, IGameStateController gameStateController)
        {
            _spawnStrategy = spawnStrategy;
            _gameStateController = gameStateController;

            _gameStateController.GameStateChanged += OnGameStateChanged;
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

            _enemies.Remove(sender);

            // Check if all enemies are dead
            if (_enemies.Count == 0)
            {
                EnemyCountReachedZero?.Invoke(this, null);
            }
        }

        private void OnGameStateChanged(GameState newState)
        {
            if(newState == GameState.Reset)
            {
                this.transform.position = Vector3.up * 0.05f;

                _spawnStrategy.ClearEnemies();

                _enemies = _spawnStrategy.SpawnEnemies();

                InitEvents();

                EnemiesSpawned?.Invoke(_enemies.Count);
            }
        }
    }
}
