namespace CSVUploaderAPI.Contract
{
    public interface IEventDispatcher<T>
    {
        public void Dispatch(IDomainEvent<T> @event);
    }
}
