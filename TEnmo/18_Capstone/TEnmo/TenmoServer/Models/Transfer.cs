using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        [Range(3001, double.PositiveInfinity, ErrorMessage = "You must have a transfer ID over 3000.")]
        public int TransferId { get; set; }

        [Required(ErrorMessage = "You must have a transfer type.")]
        [Range(1, 2, ErrorMessage = "Your transfer type ID must be 1 or 2.")]
        public int TransferTypeId { get; set; }

        [Range(1, 3, ErrorMessage = "Your transfer status ID must be 1, 2, or 3.")]
        public int TransferStatusId { get; set; }

        [Required(ErrorMessage = "You must have a sender account ID.")]
        public int AccountFrom { get; set; }

        [Required(ErrorMessage = "You must have a receiver accound ID.")]
        public int AccountTo { get; set; }

        [Range(.01, double.PositiveInfinity, ErrorMessage = "Your transfer amount cannot be 0 or negative.")]
        public decimal Amount { get; set; }

        public int Sender { get; set; }
        public string fromUser { get; set; }
        public string toUser { get; set; }
        public Transfer(int transferTypeId, int transferStatusId, int accountFrom, int accountTo, decimal amount)
        {
            this.TransferTypeId = transferTypeId;
            this.TransferStatusId = transferStatusId;
            this.AccountFrom = accountFrom;
            this.AccountTo = accountTo;
            this.Amount = amount;
        }
    }
}
