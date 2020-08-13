using NaughtyAttributes;
using SceneManagement;
using ScriptableObjects;
using UnityEngine;
using Utils;
using Zenject;

namespace Enemy
{
    public class SpecialEnemyController : MonoBehaviour, IEnemyLifeController
    {
        [SerializeField]
        private GameObject _enemyPrefab;
        [SerializeField]
        private EnemyParams _enemyParams;

        [SerializeField]
        private Vector2 _spawnDelayRange = new Vector2(10f, 30f);

        [SerializeField]
        private Vector2 _movementBoundaries = new Vector2(-1.3f, 1.3f);

        [SerializeField]
        private float _movementSpeed = 1.2f;

        [ReadOnly]
        [SerializeField]
        private float _enemySpawnDelay;

        private Vector3 _enemyMovemet;

        private bool _enemySpawned;
        private GameState _currGameState;

        private EnemyController _enemyController;

        public event EnemyKilled EnemyKilled;

        [Inject]
        private void InitializeDI(IGameStateController gameStateController)
        {
            gameStateController.GameStateChanged += ngs => _currGameState = ngs;
        }

        private void Start()
        {
            _enemySpawnDelay = UnityEngine.Random.Range(_spawnDelayRange.x, _spawnDelayRange.y);

            _enemyMovemet = UnityEngine.Random.Range(0, 2) == 1 ? Vector3.left : Vector3.right;
        }

        private void Update()
        {
            if (_currGameState != GameState.Playing)
                return;

            if(_enemySpawned)
            {
                UpdatePos();
            }
            else
            {
                UpdateSpawnTime();       
            }
        }

        private void UpdateSpawnTime()
        {
            _enemySpawnDelay -= Time.deltaTime;

            if(_enemySpawnDelay < 0)
            {
                SpawnEnemy();
            }
        }

        private void UpdatePos()
        {
            // Not proud of ... (connect to some OnDestroyByBoundary event)
            if (_enemyController == null)
            {
                _enemySpawned = false;
                return;
            }

            var currPos = this.transform.position;

            currPos -= _enemyMovemet * _movementSpeed * Time.deltaTime;

            this.transform.position = currPos;
            CheckIfLeftBoundaries(currPos);
        }

        private void CheckIfLeftBoundaries(Vector3 currPos)
        {
            if (currPos.x > _movementBoundaries.y || currPos.x < _movementBoundaries.x)
            {
                _enemySpawned = false;
                ClearSpecialEnemy();
            }
        }

        private void ClearSpecialEnemy()
        {
            if (_enemyController != null)
            {
                Destroy(_enemyController.gameObject);

                _enemySpawnDelay = UnityEngine.Random.Range(_spawnDelayRange.x, _spawnDelayRange.y);
                _enemyMovemet = UnityEngine.Random.Range(0, 2) == 1 ? Vector3.left : Vector3.right;
            }
        }

        private void SpawnEnemy()
        {
            var enemy = Instantiate(_enemyPrefab, this.transform, false);

            _enemyController = enemy.GetComponent<EnemyController>();
            _enemyController.EnemyKilled += OnEnemyKilled;

            _enemyController.InitParams(_enemyParams);

            _enemySpawned = true;

            var currPos = transform.position;
            currPos.x = _enemyMovemet.x * _movementBoundaries.y;

            this.transform.position = currPos;
        }

        private void OnEnemyKilled(EnemyController enemyController, EnemyKilledEventArgs args)
        {
            _enemyController.EnemyKilled -= OnEnemyKilled;

            EnemyKilled?.Invoke(enemyController, args);

            _enemySpawnDelay = UnityEngine.Random.Range(_spawnDelayRange.x, _spawnDelayRange.y);
            _enemyMovemet = UnityEngine.Random.Range(0, 2) == 1 ? Vector3.left : Vector3.right;

            _enemySpawned = false;
        }
    }
}