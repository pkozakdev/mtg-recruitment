using System;

namespace macrix_client
{
    public class User
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string streetName { get; set; }
        public string houseNumber { get; set; }
        public int? apartmentNumber { get; set; }
        public string postalCode { get; set; }
        public string town { get; set; }
        public string phoneNumber { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string age { get; set; }
    }
    public class BasicCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }
    public enum RestMethod
    {
        GET = 0,
        POST = 1,
        DELETE = 2
    }
   
}


