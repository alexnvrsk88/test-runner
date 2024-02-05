using System.Threading.Tasks;

namespace Runner.Core.DI
{
    public interface IInitializable
    {
        Task<bool> Initialize();
    }
}