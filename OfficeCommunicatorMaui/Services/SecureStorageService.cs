
namespace OfficeCommunicatorMaui.Services
{
    public class SecureStorageService
    {
        private int random { get; set; }

        public SecureStorageService(int random)
        {
            this.random = random;
        }

        private const string TokenKey = "JwtToken";
        private const string jack = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIyIiwiZW1haWwiOiJqYWNrQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKYWNrIiwibmJmIjoxNzM1OTA3MDE4LCJleHAiOjE3MzU5OTM0MTgsImlhdCI6MTczNTkwNzAxOH0.Z-YTKnH1nygVyjLpWSR88SVPGAoTURlt50Mj8kQW8EUjL5yuxDgKjMytha0AJdADoKvhITAsjUd0yAbkrAelZA";
        private const string john = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIxIiwiZW1haWwiOiJqb2huZG9lQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKb2hubnkiLCJuYmYiOjE3MzU5MDY5OTksImV4cCI6MTczNTk5MzM5OSwiaWF0IjoxNzM1OTA2OTk5fQ.vUybgXKMEsaBmpRAvDHnsl9c4S9KQh8L0C-LVBmrGGN3Ss2HYWaFqIWGMG7Iuz37DuV79qbLXcPcz1aMzGq8KA";
        
        public async Task SaveTokenAsync(string token)
        {
            await SecureStorage.SetAsync(TokenKey, token);
        }

        //public async Task<string?> GetTokenAsync()
        //{
        //    return await SecureStorage.GetAsync(TokenKey);
        //}

        public async Task<string?> GetTokenAsync()
        {
            return random switch
            {
                1 => jack,
                2 => john,
                _ => null
            };

            //return john;
        }



        public void RemoveToken()
        {
            SecureStorage.Remove(TokenKey);
        }
    }
}