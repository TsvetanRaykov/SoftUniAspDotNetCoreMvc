using Microsoft.EntityFrameworkCore;

namespace Vxp.Services.Data.Messages
{
    using Mapping;
    using System.Linq;
    using Vxp.Data.Common.Repositories;
    using Vxp.Data.Models;
    using System.Threading.Tasks;

    public class MessagesService : IMessagesService
    {
        private readonly IDeletableEntityRepository<Message> _messagesRepository;

        public MessagesService(IDeletableEntityRepository<Message> messagesRepository)
        {
            this._messagesRepository = messagesRepository;
        }

        public async Task<int> SendMessageAsync(string senderId, string recipientId, string message)
        {

            var newMessage = new Message
            {
                AuthorId = senderId,
                RecipientId = recipientId,
                Body = message
            };

            await this._messagesRepository.AddAsync(newMessage);


            await this._messagesRepository.SaveChangesAsync();

            return newMessage.Id;
        }

        public IQueryable<TViewModel> GetFullConversation<TViewModel>(string senderId, string recipientId)
        {
            var messagesFromDb = this._messagesRepository.AllAsNoTracking()
                .Where(m => m.AuthorId == senderId || m.AuthorId == recipientId)
                .Where(m => m.RecipientId == senderId || m.RecipientId == recipientId);


            return messagesFromDb.To<TViewModel>();
        }

        public async Task<TViewModel> GetMessageByIdAsync<TViewModel>(int messageId)
        {
            var messageFromDb = await this._messagesRepository.AllAsNoTracking()
                .Include(m => m.Author)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (messageFromDb == null)
            {
                return default;
            }

            var messageModel = AutoMapper.Mapper.Map<TViewModel>(messageFromDb);

            return messageModel;
        }
    }
}