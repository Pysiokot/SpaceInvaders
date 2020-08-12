using Projectiles;
using UnityEngine;
using UserInput;
using Utils;
using Zenject;

namespace Player
{
    public class PlayerFireController : MonoBehaviour
    {
        internal bool EnableShooting { get; set; }

        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private float _shotMinDelay = 0.4f;

        private float _projectileSpawnZOffset = 0.15f;
        private float _timer;

        IInputProxy _inputProxy;
        IGameStateController _gameStateController;
        IProjectileContainerController _projectileContainerController;

        private bool _gameStateAllowsToShoot;

        [Inject]
        private void InitializeDI(IInputProxy inputProxy, IGameStateController gameStateController, IProjectileContainerController projectileContainer)
        {
            _inputProxy = inputProxy;
            _gameStateController = gameStateController;
            _projectileContainerController = projectileContainer;

            _gameStateController.GameStateChanged += OnGameStateChanged;
        }

        private void Start()
        {
            EnableShooting = true;
        }

        void Update()
        {
            if (!EnableShooting || !_gameStateAllowsToShoot)
            {
                return;
            }

            _timer += Time.deltaTime;

            if (_timer - _shotMinDelay < 0)
                return;

            if (_inputProxy.GetButtonDown("Fire1"))
            {
                PerformShot();
                _timer = 0f;
            }
        }

        private void PerformShot()
        {
            var spawnPos = this.transform.position;

            spawnPos.z += _projectileSpawnZOffset;

            _projectileContainerController.InstanitateNewProjectile(_projectilePrefab, spawnPos, Quaternion.identity);
        }

        private void OnGameStateChanged(GameState newState)
        {
            if(newState == GameState.Playing)
            {
                _gameStateAllowsToShoot = true;
            }
            else
            {
                _gameStateAllowsToShoot = false;
            }
        }
    }
}
