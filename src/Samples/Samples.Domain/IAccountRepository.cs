using System.Threading.Tasks;

namespace Samples.Domain
{
    public interface IAccountRepository
    {
        Task CreateAccountAsync(Account account);
    }
}
