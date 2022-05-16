using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountSqlDao AccountDao;
        private readonly IUserDao UserDao;

        public AccountController(IAccountSqlDao accountDao, IUserDao userDao)
        {
            UserDao = userDao;
            AccountDao = accountDao;
        }

        [HttpGet("{userId}")]
        public decimal GetBalance(int userId)
        {
            int accountId = AccountDao.FetchAccountIDFromUserID(userId);
            Account account = AccountDao.FetchAccountByID(accountId);
            decimal balance = account.Balance;
            return balance;
        }

        [HttpPost("transfers/{sendingUserId}")]
        public bool SendFunds(int sendingUserId, int receivingUserId, decimal amount)
        {
            int fromAccount = AccountDao.FetchAccountIDFromUserID(sendingUserId);
            int toAccount = AccountDao.FetchAccountIDFromUserID(receivingUserId);
            bool success = false;
            Transfer transfer = new Transfer(2, 2, fromAccount, toAccount, amount);
            Transfer newTransfer = AccountDao.TransferCreate(transfer);
            try
            {
                AccountDao.UpdateBalance(newTransfer.AccountFrom, -newTransfer.Amount);
                AccountDao.UpdateBalance(newTransfer.AccountTo, newTransfer.Amount);
                AccountDao.UpdateTransferStatus(newTransfer.TransferId, 2);
                success = true;
            }
            catch
            {
                AccountDao.UpdateTransferStatus(newTransfer.TransferId, 3);
            }
            return success;
        }

        [HttpGet("users")]
        public List<User> FetchAllUsersIDsAndNames()
        {
            List<User> userList = UserDao.GetUsers();
            List<User> userListWithId = new List<User>();

            foreach(User user in userList)
            {
                User newUser = new User();
                newUser.UserId = user.UserId;
                newUser.Username = user.Username;
                userListWithId.Add(newUser);
            }
            return userListWithId;
        }

        [HttpGet("users/{accountId}")]
        public string FetchUserNameFromAccountID(int accountId)
        {
            return AccountDao.FetchUserNameFromAccountID(accountId);
        }

        [HttpGet("transfers")]
        public ActionResult<List<Transfer>> DisplayTransfers(int userId)
        {
            int accountId = AccountDao.FetchAccountIDFromUserID(userId);
            List<Transfer> transfers = AccountDao.FetchAllTransfersPerAccount(accountId);
            return Ok(transfers);
        }

        [HttpGet("transfers/{transferId}")]
        public Transfer DisplayATransfer(int transferId)
        {
            return AccountDao.FetchTransferByID(transferId);
        }
    }
}
