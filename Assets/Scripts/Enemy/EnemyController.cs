using System;
using System.Collections;
using Projectiles;
using ScriptableObjects;
using UnityEngine;
using Utils;
using Zenject;

namespace Enemy
{
    public class EnemyController : MonoBehaviour, IProjectileHittable, IEnemyLifeController
    {
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        public event EnemyKilled EnemyKilled;

        [SerializeField] private EnemyParams _params;
        [SerializeField] private MeshRenderer _mesh;

        [SerializeField] private GameObject _projectilePrefab;

        private IGameStateController _gameStateController;
        private IProjectileContainerController _projectileContainerController;

        private float _projectileSpawnOffsetZ = -0.11f;
        private float _shotDelay = 3f;

        private bool _isAllowedToShoot = false;
        private bool _isGameStateAllowingToShoot = false;
        private Coroutine _shootingCoroutine;

        internal void InitializeDI(IGameStateController gameStateController, IProjectileContainerController projectileContainerController)
        {
            _gameStateController = gameStateController;
            _projectileContainerController = projectileContainerController;

            _gameStateController.GameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newState)
        {
            if(newState == GameState.Playing)
            {
                _isGameStateAllowingToShoot = true;
                if(_isAllowedToShoot)
                    StartShooting();
            }
            else
            {
                _isGameStateAllowingToShoot = false;
                if(_isAllowedToShoot)
                    StopShooting();
            }
        }

        public void InitParams(EnemyParams enemyParams)
        {
            _params = enemyParams;
            
            InitGameObject();
        }

        public void AllowShooting(float maxShootDelay)
        {
            if (_shootingCoroutine != null)
                return;

            _isAllowedToShoot = true;
            _shotDelay = UnityEngine.Random.Range(0.2f, maxShootDelay);

            StartShooting();
        }

        private void StartShooting()
        {
            _shootingCoroutine = StartCoroutine(BeginShooting(_shotDelay));
        }

        public void StopShooting()
        {
            if(_shootingCoroutine != null)
            {
                StopCoroutine(_shootingCoroutine);
                _shootingCoroutine = null;
            }
        }
        
        #region Interface Methods

        public void OnProjectileEnter()
        {
            EnemyKilled?.Invoke(this, new EnemyKilledEventArgs() {Points = _params.Points});
            
            Destroy(this.gameObject);
        }
 
        #endregion
        
        private void InitGameObject()
        {
            this.transform.localScale = _params.QuadScale;
            
            _mesh.material.SetTexture(MainTex, _params.EnemyTexture);
        }

        private IEnumerator BeginShooting(float maxShootDelay)
        {
            while (_isGameStateAllowingToShoot)
            {
                yield return new WaitForSeconds(_shotDelay);

                PerformShot();

                _shotDelay = UnityEngine.Random.Range(1f, maxShootDelay);
            }
        }

        private void PerformShot()
        {
            var spawnPos = this.transform.position;
            spawnPos.z += _projectileSpawnOffsetZ;

            _projectileContainerController.InstanitateNewProjectile(_projectilePrefab, spawnPos, Quaternion.identity);
        }

        private void OnDestroy()
        {
            if(_gameStateController != null)
            {
                _gameStateController.GameStateChanged -= OnGameStateChanged;
            }
        }
    }
}
