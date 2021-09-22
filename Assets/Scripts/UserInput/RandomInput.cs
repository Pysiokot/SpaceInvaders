using UnityEngine;

namespace UserInput
{
    public class RandomInput : IInputProxy
    {
        private enum Direction
        {
            Left,
            Right
        }

        private Direction _currDir;
        private int _moveFrames;

        public RandomInput()
        {
            _currDir = Direction.Left;
            _moveFrames = Random.Range(10, 100);
        }

        public float GetAxis(string axisName)
        {
            UpdateCurrDir();

            return _currDir == Direction.Left ? -0.6f : 0.6f;
        }

        public bool GetButtonDown(string buttonName)
        {
            if (buttonName == "Fire1")
            {
                var r = Random.Range(0f, 1f);
                return r < 0.01f;
            }

            return false;
        }

        private void UpdateCurrDir()
        {
            if (_moveFrames == 0)
            {
                _currDir = _currDir == Direction.Left ? Direction.Right : Direction.Left;
                _moveFrames = Random.Range(20, 200);
            }
            
            _moveFrames -= 1;
        }
    }
}