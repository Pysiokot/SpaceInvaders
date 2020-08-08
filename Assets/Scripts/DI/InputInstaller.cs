using UserInput;
using Zenject;

namespace DI
{
    public class InputInstaller : MonoInstaller<InputInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IInputProxy>().To<KeyboardInput>().AsSingle();
        }
    }
}