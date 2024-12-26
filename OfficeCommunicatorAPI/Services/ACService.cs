using Azure.Communication;
using Azure.Communication.Identity;

namespace OfficeCommunicatorAPI.Services
{
    public class ACService
    {
        private readonly CommunicationIdentityClient _client;

        public ACService(CommunicationIdentityClient client)
        {
            _client = client;
        }

        public async Task<(string identity, string token)> CreateUserAsync()
        {
            var identityAndTokenResponse = await _client.CreateUserAndTokenAsync(scopes: new[] { CommunicationTokenScope.VoIP });

            var identity = identityAndTokenResponse.Value.User.Id;
            var token = identityAndTokenResponse.Value.AccessToken.Token;
            return (identity, token);
        }


        public async Task<string> UpdateToken(string identity)
        {
            var identityToRefresh = new CommunicationUserIdentifier(identity);
            return (await _client.GetTokenAsync(identityToRefresh, scopes: new[] { CommunicationTokenScope.VoIP })).Value.Token;
        }
    }
}
