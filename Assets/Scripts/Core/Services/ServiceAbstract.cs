using System.Threading.Tasks;

namespace Runner.Core.Services
{
    public abstract class ServiceAbstract : IService
    {
        public abstract Task<bool> Initialize();

        public virtual void Dispose()
        {
        }
    }
}
