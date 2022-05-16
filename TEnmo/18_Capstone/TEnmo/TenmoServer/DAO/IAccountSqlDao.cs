using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountSqlDao
    {
        Account FetchAccountByID(int accountId);
        Account UpdateBalance(int accountId, decimal amount);
        Transfer FetchTransferByID(int transferId);
        List<Transfer> FetchAllTransfersPerAccount(int accountId);
        Transfer TransferCreate(Transfer transfer);
        bool UpdateTransferStatus(int transferId, int transferStatusId);
        int FetchAccountIDFromUserID(int userId);
        string FetchUserNameFromAccountID(int accountId);
    }
}
