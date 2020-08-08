using System.Collections;
using Projectiles;
using ScriptableObjects;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour, IProjectileHittable, IEnemyLifeController
    {
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        public event EnemyKilled EnemyKilled;

        [SerializeField] private EnemyParams _params;
        [SerializeField] private MeshRenderer _mesh;

        [SerializeField] private GameObject _projectilePrefab;

        private float _projectileSpawnOffsetZ = -0.11f;
        private float _shotDelay;

        private Coroutine _shootingCoroutine;

        public void InitParams(EnemyParams enemyParams)
        {
            _params = enemyParams;
            
            InitGameObject();
        }

        public void AllowShooting(float maxShootDelay)
        {
            if (_shootingCoroutine != null)
                return;

            _shotDelay = UnityEngine.Random.Range(0.2f, maxShootDelay);

            _shootingCoroutine = StartCoroutine(BeginShooting(maxShootDelay));
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
            while (true)
            {
                yield return new WaitForSeconds(_shotDelay);

                PerformShot();

                _shotDelay = UnityEngine.Random.Range(0.2f, maxShootDelay);
            }
        }

        private void PerformShot()
        {
            var spawnPos = this.transform.position;
            spawnPos.z += _projectileSpawnOffsetZ;

            Instantiate(_projectilePrefab, spawnPos, Quaternion.identity);
        }
    }
}
