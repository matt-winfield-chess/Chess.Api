using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Api.Models
{
    public class PostCredentialsModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
