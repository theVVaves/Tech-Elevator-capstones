using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
    public class Account
    {
        public int UserID { get; set; }
        public int AccountID { get; set; }
        public decimal Balance { get; set; }
        public Account(int accountId, int userId, decimal balance)
        {
            this.UserID = userId;
            this.AccountID = accountId;
            this.Balance = balance;
        }
        public Account() { }
    }
}
