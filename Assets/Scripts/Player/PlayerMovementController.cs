using UnityEngine;
using UserInput;
using Zenject;

namespace Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;

        [SerializeField] private Vector2 _movementBoundries;

        internal bool EnableMovement { get; set; }

        [Inject]
        IInputProxy _inputProxy;

        private void Start()
        {
            EnableMovement = true;
        }

        // Update is called once per frame
        void Update()
        {
            if(!EnableMovement)
            {
                return;
            }

            var axisInput = _inputProxy.GetAxis("Horizontal");

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
