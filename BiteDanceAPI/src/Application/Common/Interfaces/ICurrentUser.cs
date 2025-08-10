namespace BiteDanceAPI.Application.Common.Interfaces;

public interface ICurrentUser
{
    string? Id { get; }
    string? Email { get; }
    string? Name { get; }
}
