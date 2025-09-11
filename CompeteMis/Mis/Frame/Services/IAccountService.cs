namespace Compete.Mis.Frame.Services
{
    internal interface IAccountService
    {
        Models.User? Authenticate(string tenant, string user, string password);

        byte[] GetPublicKey();
    }
}
