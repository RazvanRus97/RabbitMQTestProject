using Rabbit_Consumer_Service.Models;
using System.Threading.Tasks;

namespace Rabbit_Consumer_Service.Repositories
{
    public interface IMessageRepository
    {
        void Add(Message message);
    }
}