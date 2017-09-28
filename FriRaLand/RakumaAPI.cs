using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriRaLand {
    class RakumaAPI {
        private class RakumaRawResponse {
            public bool error = true;
            public string response = "";
        }
        public Common.Account account;
        public RakumaAPI(string email, string password) {
            this.account = new Common.Account();
            this.account.kind = Common.Account.Rakuma_Account;
            this.account.email = email;
            this.account.password = password;
        }

    }
}
