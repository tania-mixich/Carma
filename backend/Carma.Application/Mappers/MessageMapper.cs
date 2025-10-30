using Carma.Application.DTOs.Message;
using Carma.Domain.Entities;

namespace Carma.Application.Mappers;

public static class MessageMapper
{
    public static Message MapToMessage(MessageCreateDto messageCreateDto)
    {
        return new Message
        {
            Text = messageCreateDto.Message
        };
    }

    public static MessageGetDto MapToMessageGetDto(Message message)
    {
        return new MessageGetDto(
        message.User.UserName,
        message.Text,
        message.SentAt
        );
    }
}