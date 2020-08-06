using System;
using Projectiles;
using UnityEngine;

namespace Enemy
{
    internal class EnemyKilledEventArgs : EventArgs
    {
        public int Points { get; set; }
    }
    
    public class EnemyController : MonoBehaviour, IProjectileHittable
    {
        internal event EventHandler<EnemyKilledEventArgs> EnemyKilled;

        #region Interface Methods

        public void OnProjectileEnter()
        {
            EnemyKilled?.Invoke(this, new EnemyKilledEventArgs());
            
            Destroy(this.gameObject);
        #if UNITY_EDITOR
            Debug.Log("Enemy hit");
        #endif
        }
        
        #endregion
    }
}
