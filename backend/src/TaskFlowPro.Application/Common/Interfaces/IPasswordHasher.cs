namespace TaskFlowPro.Application.Common.Interfaces;

// Defined in Application, implemented in Infrastructure with BCrypt
// The handler never knows BCrypt exists
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
