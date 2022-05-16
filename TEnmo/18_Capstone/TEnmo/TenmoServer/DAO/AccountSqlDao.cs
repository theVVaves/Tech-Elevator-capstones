using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSQLDao : IAccountSqlDao
    {
        private readonly string connectionString;

        public AccountSQLDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        public Account GetAccountFromReader(SqlDataReader reader)
        {
            Account account = new Account()
            {
                AccountId = Convert.ToInt32(reader["account_id"]),
                UserId = Convert.ToInt32(reader["user_id"]),
                Balance = Convert.ToDecimal(reader["balance"])
            };
            return account;
        }
        public Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer transfer = new Transfer((int)reader["transfer_type_id"], (int)reader["transfer_status_id"], (int)reader["account_from"],
                (int)reader["account_to"], (decimal)reader["amount"])
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                Sender = (int)reader["sender"],
                toUser = (string)reader["name_to"],
                fromUser = (string)reader["name_from"]
            };
            return transfer;
        }

        public Account FetchAccountByID(int accountId)
        {
            Account dummyAccount = null;
            try 
            {
                using (SqlConnection conn = new SqlConnection(connectionString))

                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT account_id, user_id, balance FROM Account WHERE account_id = @accountId", conn);
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dummyAccount = GetAccountFromReader(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return dummyAccount;
        }
        public Account UpdateBalance(int accountId, decimal moneyToAdd)
        {
            var currentBalance = FetchAccountByID(accountId).Balance;
            var balanceAfterUpdate = currentBalance + moneyToAdd;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE account SET balance = @balanceAfterUpdate " +
                                                      "WHERE account_id = @accountId", conn);

                    cmd.Parameters.AddWithValue("@balanceAfterUpdate", balanceAfterUpdate);
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    cmd.ExecuteNonQuery();
                    return FetchAccountByID(accountId);

                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }
        public Transfer FetchTransferByID(int transferId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT transfer_id, transfer_type_id, transfer_status_id,
                                                    account_from, account_to, amount, 1 AS sender, userfrom.username AS fromuser,
                                                    userto.username AS touser FROM transfer
                                                    LEFT JOIN account AS accountfrom ON account_from = accountfrom.account_id
                                                    LEFT JOIN account AS accountto ON account_to = accountto.account_id
                                                    LEFT JOIN tenmo_user AS userfrom ON userfrom.user_id = accountfrom.user_id
                                                    LEFT JOIN tenmo_user AS userto ON userto.user_id = accountto.user_id
                                                    WHERE transfer_id = @transferId", conn);

                    cmd.Parameters.AddWithValue("@transferId", transferId);
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        return GetTransferFromReader(dataReader); 
                    }
                }
                return null;                       
            }
            catch(SqlException ex)
            {
                throw ex;
            }
        }
        public List<Transfer> FetchAllTransfersPerAccount(int accountId)
        {
            List<Transfer> listofTransfers = new List<Transfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand (@"SELECT transfer_id, transfer_type_id, transfer_status_id,
                                                    account_from, account_to, amount, 1 AS sender, userfrom.username AS fromuser,
                                                    userto.username AS touser FROM transfer
                                                    LEFT JOIN account AS accountfrom ON account_from = accountfrom.account_id
                                                    LEFT JOIN account AS accountto ON account_to = accountto.account_id
                                                    LEFT JOIN tenmo_user AS userfrom ON userfrom.user_id = accountfrom.user_id
                                                    LEFT JOIN tenmo_user AS userto ON userto.user_id = accountto.user_id
                                                    WHERE account_from = 2001
                                                    UNION
                                                    SELECT transfer_id, transfer_type_id, transfer_status_id,
                                                    account_from, account_to, amount, 1 AS sender, userfrom.username AS fromuser,
                                                    userto.username AS touser FROM transfer
                                                    LEFT JOIN account AS accountfrom ON account_from = accountfrom.account_id
                                                    LEFT JOIN account AS accountto ON account_to = accountto.account_id
                                                    LEFT JOIN tenmo_user AS userfrom ON userfrom.user_id = accountfrom.user_id
                                                    LEFT JOIN tenmo_user AS userto ON userto.user_id = accountto.user_id
                                                    WHERE account_to = 2001", conn);
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        Transfer transfer = GetTransferFromReader(dataReader);
                        listofTransfers.Add(transfer); 
                    }
                }
                return listofTransfers;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public Transfer TransferCreate(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"INSERT INTO transfer(transfer_type_id, transfer_status_id, account_from, account_to, amount)
                                                      OUTPUT inserted.transfer_id
                                                      VALUES (@transferTypeId, @transferStatusId, @accountFrom, @accountTo, @amount)", conn);
                    cmd.Parameters.AddWithValue("@transferTypeId", transfer.TransferTypeId);
                    cmd.Parameters.AddWithValue("@transferStatusId", transfer.TransferStatusId);
                    cmd.Parameters.AddWithValue("@accountFrom", transfer.AccountFrom);
                    cmd.Parameters.AddWithValue("@accountTo", transfer.AccountTo);
                    cmd.Parameters.AddWithValue("@amount", transfer.Amount);
                    transfer.TransferId = (int)cmd.ExecuteScalar();
                    return transfer;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }
        public bool UpdateTransferStatus(int transferId, int transferStatusId)
        {
            bool updateCheck = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE transfer SET transfer_status_id = @transferStatusId " +
                                                   "WHERE transfer_type_id = @transferId", conn);

                    cmd.Parameters.AddWithValue("@transferId", transferId);
                    cmd.Parameters.AddWithValue("@transferStatusId", transferStatusId);
                    updateCheck = true;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return updateCheck;
        }
        public int FetchAccountIDFromUserID(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT account_id FROM account " +
                                                     "JOIN tenmo_user  ON tenmo_user.user_id = account.user_id " +
                                                     "WHERE tenmo_user.user_id = @userId", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    int accountId = (int)cmd.ExecuteScalar();
                    return accountId;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }
        public string FetchUserNameFromAccountID(int accountId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT tu.username FROM tenmo_user " +
                                                    "JOIN account a ON a.user_id = tu.user_id " +
                                                     "WHERE a.account_id = @accountId", conn);
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    string username = (string)cmd.ExecuteScalar();
                    return username;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }

        }
    }
}