using UnityEngine;

namespace SceneManagement
{
    public class LevelBoundariesController : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            Destroy(other.gameObject);
        }
    }
}