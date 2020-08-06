using UnityEngine;

namespace Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;

        [SerializeField] private Vector2 _movementBoundries;
    
        // Update is called once per frame
        void Update()
        {
            var axisInput = Input.GetAxis("Horizontal");

            if (axisInput == 0)
            {
                return;
            }

            var currPlayerPos = this.transform.position;

            currPlayerPos.x = Mathf.Clamp(currPlayerPos.x + (_movementSpeed * axisInput * Time.deltaTime),
                _movementBoundries.x, _movementBoundries.y);

            this.transform.position = currPlayerPos;
        }
    }
}
