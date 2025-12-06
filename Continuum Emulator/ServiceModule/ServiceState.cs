using Microsoft.Xna.Framework.Input;

namespace Continuum93.ServiceModule
{
    public class ServiceState
    {
        public bool ServiceMode = false;
        public Keys ServiceKey = Keys.F1;
        public bool UseServiceView;

        public void ToggleServiceMode()
        {
            ServiceMode = !ServiceMode;
        }
    }
}
