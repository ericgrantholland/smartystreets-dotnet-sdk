using System.Threading.Tasks;

namespace SmartyStreets.USStreetApi
{
    // marker interface for easy dependency injection and unit test mocking
    public interface IUSStreetClient : IClient<Lookup>
    {
        Task SendAsync(Batch batch);
    }
}