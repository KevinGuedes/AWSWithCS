using FluentResults;
using MediatR;

namespace Customers.Messages;

public interface IMessage : IRequest<Result>
{
}
