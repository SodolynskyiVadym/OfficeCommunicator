namespace OfficeCommunicatorAPI.Services.Checkers
{
    public interface IChecker
    {
        public Task<bool> CheckPermissionUser(int userId, int chatId);
    }
}
