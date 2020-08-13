using UnityEngine;

namespace SceneManagement
{
    public delegate void EnemyReachedTarget();

    public interface IEnemyTargetController
    {
        event EnemyReachedTarget EnemyReachedTarget;
    }

    public class EnemyTargetController : MonoBehaviour, IEnemyTargetController
    {
        public event EnemyReachedTarget EnemyReachedTarget;

        private int _layerMask;

        private void Start()
        {
            _layerMask = LayerMask.NameToLayer("Enemy");
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer == _layerMask)
            {
                EnemyReachedTarget?.Invoke();
            }
        }
    }
}