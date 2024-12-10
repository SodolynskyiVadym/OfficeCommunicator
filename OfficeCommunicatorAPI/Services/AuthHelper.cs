using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using OfficeCommunicatorAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace OfficeCommunicatorAPI.Services
{
    public class AuthHelper
    {
        private readonly string jwtKey;
        private readonly string passwordKey;
        private readonly string possibleValuePassword = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        public AuthHelper(string jwtKey, string passwordKey)
        {
            this.jwtKey = jwtKey;
            this.passwordKey = passwordKey;
        }

        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = passwordKey + Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
        }


        public string CreateToken(User user)
        {
            Claim[] claims =
            [
                new("userId", user.Id.ToString()),
                new("email", user.Email),
                new("nickname", user.UniqueName)
            ];

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }


        public byte[][] EncryptUserPassword(string password)
        {
            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(password, passwordSalt);

            return [passwordHash, passwordSalt];
        }


        public bool CheckPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            byte[] passwordHashToCheck = GetPasswordHash(password, passwordSalt);
            for (var index = 0; index < passwordHashToCheck.Length; index++) if (passwordHashToCheck[index] != passwordHash[index]) return false;
            return true;
        }

        public bool IsEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

    }
}
