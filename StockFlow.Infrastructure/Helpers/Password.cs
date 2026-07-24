using Application.Interfaces.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Infrastructure.Helpers
{
    public class Password : IPassword
    {
        public bool Validate(string hash, string password)
            => BCrypt.Net.BCrypt.Verify(password, hash);

        public string Hash(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);
    }
}
