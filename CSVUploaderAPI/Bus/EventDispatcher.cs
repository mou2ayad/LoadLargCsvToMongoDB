using System.Threading.Tasks;
using CSVUploaderAPI.Contract;
using Microsoft.Extensions.Logging;
using SlimMessageBus;

namespace CSVUploaderAPI.Bus
{
    public class EventDispatcher<T> : IEventDispatcher<T>
    {
        private readonly IMessageBus _bus;
        private readonly ILogger<EventDispatcher<T>> _logger;

        public EventDispatcher(IMessageBus bus, ILogger<EventDispatcher<T>> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public async Task Dispatch(IDomainEvent<T> @event)
        {
            _logger.LogInformation($"Dispatching a new event {@event.GetEvent().GetType().Name}");
            await _bus.Publish(@event.GetEvent());
        }
    }
}
