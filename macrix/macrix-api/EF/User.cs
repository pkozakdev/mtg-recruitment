using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace macrix_api.EF
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetName { get; set; }
        public string HouseNumber { get; set; }
        public int? ApartmentNumber { get; set; }
        public string PostalCode { get; set; }
        public string Town { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Age => this.GetAge();

        public string GetAge()
        {
            string age = (DateTime.Now.Year - this.DateOfBirth.Year).ToString();
            return age;
        }
    }

    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FirstName);
            builder.Property(x => x.LastName);
            builder.Property(x => x.StreetName);
            builder.Property(x => x.HouseNumber);
            builder.Property(x => x.ApartmentNumber);
            builder.Property(x => x.PostalCode);
            builder.Property(x => x.Town);
            builder.Property(x => x.PhoneNumber);
            builder.Property(x => x.DateOfBirth);
        }
    }
}