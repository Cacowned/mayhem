
namespace MayhemCore
{
    public abstract class ReactionBase: ModuleBase
    {
        public ReactionBase(string name, string description)
            :base(name, description)
        {
        }

        public abstract void Perform();
    }
}
