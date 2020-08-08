using UnityEngine;
using UserInput;
using Zenject;

namespace Player
{
    public class PlayerFireController : MonoBehaviour
    {
        internal bool EnableShooting { get; set; }

        [SerializeField] private GameObject _projectilePrefab;

        private float _projectileSpawnZOffset = 0.15f;

        [Inject]
        IInputProxy _inputProxy;

        private void Start()
        {
            EnableShooting = true;
        }

        void Update()
        {
            if (!EnableShooting)
            {
                return;
            }

            if (_inputProxy.GetButtonDown("Fire1"))
            {
                PerformShot();
            }
        }

        private void PerformShot()
        {
            var spawnPos = this.transform.position;

            spawnPos.z += _projectileSpawnZOffset;
            
            Instantiate(_projectilePrefab, spawnPos, Quaternion.identity);
        }
    }
}
