namespace UserInput
{
    public interface IInputProxy
    {
        float GetAxis(string axisName);
        bool GetButtonDown(string buttonName);
    }
}