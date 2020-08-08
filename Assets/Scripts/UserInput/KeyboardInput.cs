using UnityEngine;

namespace UserInput
{
    public class KeyboardInput : IInputProxy
    {
        public float GetAxis(string axisName)
        {
            return Input.GetAxis(axisName);
        }

        public bool GetButtonDown(string buttonName)
        {
            return Input.GetButtonDown(buttonName);
        }
    }
}