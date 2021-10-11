using System.Threading.Tasks;

namespace SmartyStreets.USZipCodeApi
{
    // marker interface for easy dependency injection and unit test mocking
    public interface IUSZipCodeClient : IClient<Lookup>
    {
        Task SendAsync(Batch batch);
    }
}