namespace CSVUploaderAPI.Contract
{
    public interface IDomainEvent<out T>
    {
        T GetEvent();
    }
}
