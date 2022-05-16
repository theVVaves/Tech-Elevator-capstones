using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Exceptions;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class AccountApiService : AuthenticatedApiService
    {
        private readonly string API_URL = "https://localhost:44315";
        public AccountApiService(string apiUrl) : base(apiUrl) => this.API_URL = apiUrl;
        public AccountApiService() { }
        public Account GetAccount()
        {
            Account userAccount = new Account();
            RestRequest request = new RestRequest(API_URL + "/account");
            IRestResponse<Account> response = client.Get<Account>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;
        }
        public decimal? GetBalance()
        {
            RestRequest request = new RestRequest(API_URL + "account/" + user.UserId);
            IRestResponse<decimal> response = client.Get<decimal>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;
        }
        public Transfer GetTransferInfoById(int transferId)
        {
            Transfer transfer = new Transfer();
            RestRequest request = new RestRequest(API_URL + "account/transfers/" + transferId);
            IRestResponse<Transfer> response = client.Get<Transfer>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;

        }
        public string GetUsername(int accountId)
        {
            RestRequest request = new RestRequest(API_URL + "users/" + accountId);
            IRestResponse<string> response = client.Get<string>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return "";

        }
        public List<ApiUser> GetAllUsers()
        {
            List<ApiUser> users = new List<ApiUser>();
            RestRequest request = new RestRequest(API_URL + "account/users");
            IRestResponse<List<ApiUser>> response = client.Get<List<ApiUser>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;
        }
        public List<Transfer> GetPastTransfers()
        {
            List<Transfer> users = new List<Transfer>();
            RestRequest request = new RestRequest(API_URL + "account/transfers/?userId=" + user.UserId);
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;

        }
        public void TransferFunds(int userId, decimal transferAmount)
        {
            RestRequest request = new RestRequest(API_URL + "account/transfers/" + user.UserId + "?receivingUserId=" + userId + "&amount=" + transferAmount);
            IRestResponse response = client.Post(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ErrorResponse(response);
            }
            else
            {
                Console.WriteLine("Congrats! You have completed a transfer.");
            }
        }
        private void ErrorResponse(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new NoResponseException("You were not able to reach the server.", response.ErrorException);
            }
            else if (!response.IsSuccessful)
            {

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedException("You must be logged in to perform the task.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new ForbiddenException("You lack sufficient permission to perform the task.");
                }
                else
                {
                    throw new NonSuccessException((int)response.StatusCode);
                }
            }
        }
    }
}