using Projectiles;
using UnityEngine;

namespace Player
{

    public class PlayerController : MonoBehaviour, IProjectileHittable, IPlayerLifeController
    {
        public event PlayerHit PlayerHit;
        public event PlayerLifeReachedZero PlayerLifeReachedZero;

        [SerializeField] private PlayerFireController _fireController;
        [SerializeField] private PlayerMovementController _movementController;
        [SerializeField] private int _playerLifes;

        public int LifesLeft => _playerLifes;

        public void OnProjectileEnter()
        {
            _playerLifes -= 1;

            PlayerHit?.Invoke(this);

            _fireController.EnableShooting = false;
            _movementController.EnableMovement = false;

            Invoke("Respawn", 1f);
        }
        
        public void Respawn()
        {
            _fireController.EnableShooting = true;
            _movementController.EnableMovement = true;
        }
    }
}