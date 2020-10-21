using Chess.Api.Utils.Interfaces;
using System;
using System.Linq;

namespace Chess.Api.Utils
{
    public class StringIdGenerator : IStringIdGenerator
    {
        private const string VALID_CHARACTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private readonly Random _random = new Random();

        public string GenerateId(int length = 10)
        {
            var characters = new char[length];

            for(int i = 0; i < length; i++)
            {
                characters[i] = VALID_CHARACTERS[_random.Next(0, VALID_CHARACTERS.Length)];
            }

            return new string(characters);
        }
    }
}
