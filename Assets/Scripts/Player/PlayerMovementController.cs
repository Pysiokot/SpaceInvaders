using UnityEngine;
using UserInput;
using Utils;
using Zenject;

namespace Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;

        [SerializeField] private Vector2 _movementBoundries;

        internal bool EnableMovement { get; set; }

        private bool _gameStateAllowsToMove;

        IInputProxy _inputProxy;
        IGameStateController _gameStateController;

        [Inject]
        private void InitializeDI(IInputProxy inputProxy, IGameStateController gameStateController)
        {
            _inputProxy = inputProxy;
            _gameStateController = gameStateController;

            _gameStateController.GameStateChanged += OnGameStateChanged;
        }

        private void Start()
        {
            EnableMovement = true;
        }

        // Update is called once per frame
        void Update()
        {
            if(!EnableMovement || !_gameStateAllowsToMove)
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

        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.Playing)
            {
                _gameStateAllowsToMove = true;
            }
            else
            {
                _gameStateAllowsToMove = false;
            }
        }
    }
}
