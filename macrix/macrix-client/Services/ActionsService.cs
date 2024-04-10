using Figgle;
using macrix_client.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace macrix_client.Controllers
{
    public interface IActionsService
    {
        void Hello();
        void RenderDashboard(bool reactive = true);
        List<User> GetUsers();
        void PrintLine();
        void PrintRow(params string[] columns);
        string Align(string text, int width);
    }
    internal class ActionsService : IActionsService
    {
        private readonly ILogger<ActionsService> _logger;
        private readonly IMacrixAPIService _macrixApiService;
        private readonly IConfiguration _configuration;
        private readonly int _width;
        private List<User> Users;


        public ActionsService(ILoggerFactory loggerFactory, IMacrixAPIService macrixAPIService)
        {
            _logger = loggerFactory.CreateLogger<ActionsService>();
            _macrixApiService = macrixAPIService;
            _configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build();
            _width = _configuration.GetValue<int>("TableWidth");

        }
        public void Hello()
        {
            Console.WriteLine(FiggleFonts.Ogre.Render("macrix"));
            Console.WriteLine(FiggleFonts.Straight.Render("technology group"));
            Console.WriteLine("");
            Console.WriteLine("Please use the full-screen mode");

        }

        public void RenderDashboard(bool interactive = true)
        {
            Users = GetUsers();
            if (Users.Any())
            {
                Console.WriteLine("");
                Console.WriteLine("Users in the system:");
                Console.WriteLine("");
                Console.WriteLine("");
                PrintLine();
                PrintRow("Id", "First Name", "Last Name", "Street Name", "House No", "Appt No", "Postal Code", "Town", "Phone No", "DOB", "Age");
                PrintLine();
                foreach (var x in Users)
                {
                    PrintRow(x.id.ToString(), x.firstName, x.lastName, x.streetName, x.houseNumber, x.apartmentNumber.ToString(), x.postalCode, x.town, x.phoneNumber, x.dateOfBirth.ToShortDateString(), x.age);

                }
                PrintLine();
                Console.WriteLine();
                if (interactive)
                {
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("Press e to edit, press any other key to refresh");
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("No users in the database");
                if (interactive)
                {
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("Press e to edit, press any other key to refresh");
                }
            }
            if (interactive)
            {
                //interaction
                do
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey();
                        if (key.Key == ConsoleKey.E)
                        {
                            RenderOptions();
                            _logger.LogDebug("Key pressed: " + key.Key);
                        }
                        else
                        {
                            Console.Clear();
                            RenderDashboard();
                        }

                    }
                } while (true);
            }

        }
        public void RenderOptions()
        {
            var correctOptionPicked = false;
            Console.Clear();
            Console.WriteLine("Choose one of the options:");
            Console.WriteLine("A: Add User");
            Console.WriteLine("B: Edit User");
            Console.WriteLine("C: Delete User");
            Console.WriteLine("D: Go back to the Dashboard");
            _logger.LogDebug("Options rendered");
            while (correctOptionPicked == false)
            {
                var pickedOption = Console.ReadKey();
                if (pickedOption.Key == ConsoleKey.A)
                {
                    correctOptionPicked = true;
                    AddUser();
                }
                else if (pickedOption.Key == ConsoleKey.B)
                {
                    correctOptionPicked = true;
                    EditUser();

                }
                else if (pickedOption.Key == ConsoleKey.C)
                {
                    correctOptionPicked = true;
                    DeleteUser();

                }
                else if (pickedOption.Key == ConsoleKey.D)
                {
                    correctOptionPicked = true;
                    Console.Clear();
                    RenderDashboard();

                }
                else
                {
                    Console.WriteLine("Please choose an option from the list");
                }

            }

        }
        public void AddUser()
        {

            Console.Clear();
            User newUser = GetUserInfo();
            _macrixApiService.CallRestMethod(RestMethod.POST, 0, newUser);
            Console.Clear();
            Thread.Sleep(1500);
            Console.Clear(); _logger.LogDebug("User Added " + newUser);
            RenderDashboard();

        }
        public void EditUser()
        {
            Console.Clear();
            RenderDashboard(false);
            Console.WriteLine("");
            Console.WriteLine("Provide userId for the record you'd like to edit, accept with enter:");
            var userId = GetValue();
            Console.Clear();
            User newUser = GetUserInfo();
            _macrixApiService.CallRestMethod(RestMethod.POST, Convert.ToInt32(userId), newUser);
            _logger.LogDebug("User Edited " + newUser);
            Console.Clear();
            Thread.Sleep(1000);
            Console.Clear();
            RenderDashboard();

        }
        public void DeleteUser()
        {
            Console.Clear();
            RenderDashboard(false);
            Console.WriteLine("");
            Console.WriteLine("Provide userId for the record you'd like to delete, accept with enter:");
            var userId = GetValue(false, true, false, true);
            Console.Clear();
            _macrixApiService.CallRestMethod(RestMethod.DELETE, Convert.ToInt32(userId));
            _logger.LogDebug("User Edited: Id " + userId);
            Console.Clear();
            Thread.Sleep(1000);
            Console.Clear();
            RenderDashboard();

        }
        public User GetUserInfo()
        {
            User newUser = new User();
            Console.WriteLine("");
            Console.WriteLine("Provide user details, accept with enter");
            Console.WriteLine("Fields marked with * are mandatory");
            Console.WriteLine("");
            Console.WriteLine("First Name*");
            newUser.firstName = GetValue();
            Console.WriteLine("Last Name*");
            newUser.lastName = GetValue();
            Console.WriteLine("Street Name*");
            newUser.streetName = GetValue();
            Console.WriteLine("House Number*");
            newUser.houseNumber = GetValue();
            Console.WriteLine("Apartment Number");
            string optionalApartmentNo = GetValue(false, true, true);
            if (!String.IsNullOrEmpty(optionalApartmentNo))
            {
                newUser.apartmentNumber = Convert.ToInt32(optionalApartmentNo);
            }
            Console.WriteLine("Postal Code*");
            newUser.postalCode = GetValue();
            Console.WriteLine("Town*");
            newUser.town = GetValue();
            Console.WriteLine("Phone Number*");
            newUser.phoneNumber = GetValue();
            Console.WriteLine("Date of Birth (dd/MM/yyyy)*");
            newUser.dateOfBirth = DateTime.ParseExact(GetValue(true), "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
            return newUser;
        }
        public string GetValue(bool dateTime = false, bool integer = false, bool allowEmpty = false, bool checkForUser = false)
        {
            if (allowEmpty)
            {
                string value = Console.ReadLine();
                if (value != "")
                {
                    if (dateTime == true)
                    {
                        DateTime readDateTime;
                        while (!DateTime.TryParseExact(value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out readDateTime))
                        {
                            Console.WriteLine("Invalid date of birth, please provide one adhering to the guidelines");
                            value = Console.ReadLine();
                        }
                        return value;
                    }
                    else if (integer == true)
                    {
                        var isInteger = false;
                        while (String.IsNullOrEmpty(value) || value == "default" || isInteger == false)
                        {
                            var isNumeric = int.TryParse(value, out _) || value == "";
                            while (isNumeric == false)
                            {
                                Console.WriteLine("Provide a numeric value");
                                value = Console.ReadLine();
                                isNumeric = int.TryParse(value, out _);
                            }
                            if (checkForUser && !Users.Any(x => x.id == Convert.ToInt32(value)))
                            {
                                Console.WriteLine("User doesn't exist");
                                value = "default";
                            }
                            isInteger = true;

                        }
                        return value;
                    }
                    else
                    {
                        while (String.IsNullOrEmpty(value))
                        {

                            Console.WriteLine("Value cannot be empty");
                            value = Console.ReadLine();

                        }
                        return value;
                    }
                }
                else
                {
                    return value;
                }
            }
            else
            {
                if (dateTime == true)
                {
                    DateTime readDateTime;
                    string value = Console.ReadLine();
                    while (!DateTime.TryParseExact(value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out readDateTime))
                    {
                        Console.WriteLine("Invalid date of birth, please provide one adhering to the guidelines");
                        value = Console.ReadLine();
                    }
                    return value;
                }
                else if (integer == true)
                {
                    var value = "default";
                    var isInteger = false;
                    while (String.IsNullOrEmpty(value) || value == "default" || isInteger == false)
                    {
                        value = Console.ReadLine();
                        if (value == "default" || String.IsNullOrEmpty(value))
                        {
                            Console.WriteLine("Value cannot be empty");
                            value = "default";
                        }
                        else
                        {
                            var isNumeric = int.TryParse(value, out _);
                            while (isNumeric == false)
                            {
                                Console.WriteLine("Provide a numeric value");
                                value = Console.ReadLine();
                                isNumeric = int.TryParse(value, out _);
                            }
                            if (checkForUser && !Users.Any(x => x.id == Convert.ToInt32(value)))
                            {
                                Console.WriteLine("User doesn't exist");
                                value = "default";
                            }
                            isInteger = true;

                        }

                    }
                    return value;
                }
                else
                {
                    var value = "default";
                    while (String.IsNullOrEmpty(value) || value == "default")
                    {
                        value = Console.ReadLine();
                        if (value == "default" || String.IsNullOrEmpty(value))
                        {
                            Console.WriteLine("Value cannot be empty");
                        };
                    }
                    return value;
                }
            }


        }
        public List<User> GetUsers()
        {
            var users = _macrixApiService.CallRestMethod(RestMethod.GET, 0).Result;
            if (users != "[]")
            {
                return JsonSerializer.Deserialize<IList<User>>(users).ToList();

            }
            else
            {
                return new List<User>();
            }
        }

        public void PrintLine()
        {
            Console.WriteLine(new string('~', _width));
        }

        public void PrintRow(params string[] columns)
        {
            int width = (_width - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += Align(column, width) + "|";
            }

            Console.WriteLine(row);
        }
        public string Align(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
    }
}
