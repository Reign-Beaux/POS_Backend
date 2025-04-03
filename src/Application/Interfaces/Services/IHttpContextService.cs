namespace Application.Interfaces.Services
{
    public interface IHttpContextService
    {
        void SetCookie(string name, string value, int duration);
    }
}
