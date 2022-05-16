using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Account
    {
        [Required(ErrorMessage = "You must have a user ID.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "You must have an account ID.")]
        [Range(2001, double.PositiveInfinity, ErrorMessage = "Yout account ID must be number over 2000.")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "You must have a balance.")]
        [Range(0, double.PositiveInfinity, ErrorMessage = "Your balance cannot be negative.")]
        public decimal Balance { get; set; }

        public Account(int accountId, int userId, decimal balance)
        {
            this.AccountId = accountId;
            this.UserId = userId;
            this.Balance = balance;
        }
        public Account() { }
    }
}
