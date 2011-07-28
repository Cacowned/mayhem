
using MayhemCore;
using MayhemCore.ModuleTypes;
namespace MayhemWpf
{
    internal static class MayhemInstance
    {
        private static Mayhem<IWpf> _instance;

        public static Mayhem<IWpf> Instance
        {
            get
            {
                if (_instance == null) {
                    _instance = new Mayhem<IWpf>();
                }

                return _instance;
            }
        }
    }
}
