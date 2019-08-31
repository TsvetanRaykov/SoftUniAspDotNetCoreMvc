namespace Vxp.Services.Data.Messages
{
    using System.Linq;
    using System.Threading.Tasks;

    public interface IMessagesService
    {
        Task<int> SendMessageAsync(string senderId, string recipientId, string message);
        IQueryable<TViewModel> GetFullConversation<TViewModel>(string senderId, string recipientId);
        Task<TViewModel> GetMessageByIdAsync<TViewModel>(int messageId);
    }
}