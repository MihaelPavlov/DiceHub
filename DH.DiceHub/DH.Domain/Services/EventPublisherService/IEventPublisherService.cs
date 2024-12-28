namespace DH.Domain.Services.Publisher;

public interface IEventPublisherService
{
    Task PublishClubActivityDetectedMessage();
}
