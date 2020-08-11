using System.Collections.Generic;
using UnityEngine;
using Utils;
using Zenject;

namespace SceneManagement
{
    public class ObstaclesController : MonoBehaviour
    {
        private readonly int _obstacles_count = 4;

        [SerializeField] private GameObject _obstaclePrefab;

        private IGameStateController _gameStateController;

        private IList<GameObject> _obstacles = new List<GameObject>();
        private Vector2 _obstaclesBoundaries = new Vector2(-0.8f, 0.8f);

        [Inject]
        private void InitializeDI(IGameStateController gameStateController)
        {
            _gameStateController = gameStateController;

            _gameStateController.GameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newState)
        {
            if(newState == GameState.Reset)
            {
                RemoveObstacles();

                _obstacles.Clear();

                SpawnObstacles();
            }
        }

        private void SpawnObstacles()
        {
            var dst = (_obstaclesBoundaries.y - _obstaclesBoundaries.x) / (_obstacles_count - 1f);

            for (int i = 0; i < _obstacles_count; i++)
            {
                var obstacleSpawnPos = Vector3.right * (_obstaclesBoundaries.x + (dst * i));

                var newGo = Instantiate(_obstaclePrefab, obstacleSpawnPos + this.transform.position, Quaternion.identity, this.transform);

                _obstacles.Add(newGo);
            }
        }

        private void RemoveObstacles()
        {
            for (int i = 0; i < _obstacles.Count; i++)
            {
                if(_obstacles[i] != null)
                {
                    Destroy(_obstacles[i]);
                }
            }

            _obstacles.Clear();
        }
    }
}
