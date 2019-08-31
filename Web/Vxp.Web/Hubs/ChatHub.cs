namespace Vxp.Web.Hubs
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;
    using ViewModels.Messages;
    using Vxp.Services.Data.Messages;

    public class ChatHub : Hub
    {

        private readonly IMessagesService _messagesService;

        public ChatHub(IMessagesService messagesService)
        {
            this._messagesService = messagesService;
        }

        public async Task TakeMessagesUpdate(int messageId)
        {
            var message = await this._messagesService.GetMessageByIdAsync<MessageInputModel>(messageId);
            await this.Clients.User(message.RecipientId).SendAsync("UpdateChatWindow", message.ToJson());
            await this.Clients.Caller.SendAsync("UpdateChatWindow", message.ToJson());
        }

        public async Task LoadConversation(string recipientId)
        {
            var currentUser = this.Context.UserIdentifier;
            var messages =
                this._messagesService.GetFullConversation<MessageInputModel>(this.Context.UserIdentifier,
                    recipientId);

            await this.Clients.Caller.SendAsync("LoadConversation", messages.ToJson());
        }
    }
}