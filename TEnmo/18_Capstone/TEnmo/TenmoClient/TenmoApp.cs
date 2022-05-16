using System;
using System.Collections.Generic;
using TenmoClient.Models;
using TenmoClient.Services;

namespace TenmoClient
{
    public class TenmoApp
    {
        private readonly TenmoConsoleService console = new TenmoConsoleService();
        private readonly ConsoleService primConsole = new ConsoleService();
        private readonly AccountApiService accountService;
        public TenmoApp(string apiUrl) => accountService = new AccountApiService(apiUrl);

        public void Run()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                // The menu changes depending on whether the user is logged in or not
                if (accountService.IsLoggedIn)
                {
                    keepGoing = RunAuthenticated();
                }
                else // User is not yet logged in
                {
                    keepGoing = RunUnauthenticated();
                }
            }
        }

        private bool RunUnauthenticated()
        {
            console.PrintLoginMenu();
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 2, 1);
            while (true)
            {
                if (menuSelection == 0)
                {
                    return false;   // Exit the main menu loop
                }

                if (menuSelection == 1)
                {
                    // Log in
                    Login();
                    return true;    // Keep the main menu loop going
                }

                if (menuSelection == 2)
                {
                    // Register a new user
                    Register();
                    return true;    // Keep the main menu loop going
                }
                console.PrintError("Invalid selection. Please choose an option.");
                console.Pause();
            }
        }

        private bool RunAuthenticated()
        {
            console.PrintMainMenu(accountService.Username);
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 6);
            if (menuSelection == 0)
            {
                // Exit the loop
                return false;
            }

            if (menuSelection == 1)
            {
                // View your current balance
                decimal? balance = accountService.GetBalance();
                Console.WriteLine("Your balance is: TE$" + balance);
                console.Pause();
            }

            if (menuSelection == 2)
            {
                // View your past transfers
                List<Transfer> transfers = accountService.GetPastTransfers();
                Console.WriteLine("++++++++++++++");
                Console.WriteLine("Transfers:");
                Console.WriteLine("ID       From/To         Amount");
                Console.WriteLine("++++++++++++++");
                foreach (Transfer transfer in transfers)
                {
                    if(transfer.Sender == 0)
                    {
                        Console.WriteLine($"{transfer.TransferId}       {transfer.fromUser}         {transfer.Amount}");
                    }
                    else if(transfer.Sender == 1)
                    {
                        Console.WriteLine($"{transfer.TransferId}       {transfer.toUser}         {transfer.Amount}");
                    }
                }
                int transferId = primConsole.PromptForInteger("Please specify the transfer ID you wish to view: ");
            }

            if (menuSelection == 3)
            {
                // View your pending requests
            }

            if (menuSelection == 4)
            {
                // Send TE bucks
                List<ApiUser> users = accountService.GetAllUsers();
                if (users!= null && users.Count > 0)
                {
                    Console.WriteLine("++++++++++++++");
                    Console.WriteLine("Users:");
                    Console.WriteLine("ID       Name");
                    Console.WriteLine("++++++++++++++");
                    foreach (ApiUser user in users)
                    {
                        Console.WriteLine($"{user.UserId}       {user.Username}");
                    }
                    Console.WriteLine("++++++++++++++");
                    int userId = primConsole.PromptForInteger("Please specify the user ID you wish to send to: ");
                    bool idCheck = false;
                    while (idCheck == false && userId != 0)
                    {
                        foreach(ApiUser user in users)
                        {
                            if(user.UserId.Equals(userId))
                            {
                                Console.WriteLine("Thank you for not being a pr*ck and giving us a valid ID :)");
                                idCheck = true;
                                break;
                            }
                        }
                        if (idCheck == false)
                        {
                                int newId = primConsole.PromptForInteger("Please specify a valid ID, you heathen: ");
                        }
                    }
                    decimal transAmount = primConsole.PromptForDecimal("Please specify the amount of TE$ to be transferred");
                    if (transAmount != 0)
                    {
                        accountService.TransferFunds(userId, transAmount);
                    }
                }
            }

            if (menuSelection == 5)
            {
                // Request TE bucks
            }

            if (menuSelection == 6)
            {
                // Log out
                accountService.Logout();
                console.PrintSuccess("You are now logged out");
            }

            return true;    // Keep the main menu loop going
        }

        private void Login()
        {
            LoginUser loginUser = console.PromptForLogin();
            if (loginUser == null)
            {
                return;
            }

            try
            {
                ApiUser user = accountService.Login(loginUser);
                if (user == null)
                {
                    console.PrintError("Login failed.");
                }
                else
                {
                    console.PrintSuccess("You are now logged in");
                }
            }
            catch (Exception)
            {
                console.PrintError("Login failed.");
            }
            console.Pause();
        }

        private void Register()
        {
            LoginUser registerUser = console.PromptForLogin();
            if (registerUser == null)
            {
                return;
            }
            try
            {
                bool isRegistered = accountService.Register(registerUser);
                if (isRegistered)
                {
                    console.PrintSuccess("Registration was successful. Please log in.");
                }
                else
                {
                    console.PrintError("Registration was unsuccessful.");
                }
            }
            catch (Exception)
            {
                console.PrintError("Registration was unsuccessful.");
            }
            console.Pause();
        }
    }
}
