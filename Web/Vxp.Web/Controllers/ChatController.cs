namespace Vxp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Messages;
    using System.Threading.Tasks;
    using Vxp.Services.Data.Messages;
    using System.Security.Claims;


    [Route("[controller]")]
    [Authorize]
    public class ChatController : BaseController
    {
        private readonly IMessagesService _messagesService;

        public ChatController(IMessagesService messagesService)
        {
            this._messagesService = messagesService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage([FromBody]MessageInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {
                var messageId = await this._messagesService
                     .SendMessageAsync(
                         this.User.FindFirstValue(ClaimTypes.NameIdentifier),
                         inputModel.RecipientId,
                         inputModel.Message);
                return this.Ok(messageId);
            }

            return this.BadRequest();
        }
    }
}