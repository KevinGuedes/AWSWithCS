using FluentResults;
using MediatR;

namespace Shared.Customers.Messages;

public interface IMessage : IRequest<Result>
{
}
