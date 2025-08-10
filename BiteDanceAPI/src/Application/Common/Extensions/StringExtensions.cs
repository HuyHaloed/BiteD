using System;
using System.Linq;

namespace BiteDanceAPI.Application.Common.Extensions
{
    public static class StringExtensions
    {
        private static readonly Random _random = new();

        public static string GenerateRandomString(this string str, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(
                Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray()
            );
        }
    }
}
