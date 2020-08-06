using System;
using UnityEngine;

namespace Player
{
    public class PlayerFireController : MonoBehaviour
    {
        [SerializeField] private GameObject _projectilePrefab;

        private float _projectileSpawnZOffset = 0.15f;
        
        void Update()
        {
            if (Input.GetButtonDown("Fire1"))
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
