using AppServicios.Application.Interfaces;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }
    }
}