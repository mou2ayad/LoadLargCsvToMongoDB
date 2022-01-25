using System.Threading.Tasks;

namespace CSVUploaderAPI.Contract
{
    public interface IEventDispatcher<T>
    {
        public Task Dispatch(IDomainEvent<T> @event);
    }
}
