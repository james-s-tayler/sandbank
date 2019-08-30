using System;
using System.ComponentModel.DataAnnotations;
using Core;

namespace Domain.User
{
    public class RegisterUserRequest : CreateModel<User, int>
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(25)] 
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }

        [MaxLength(250)] 
        public string Address { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Country { get; set; }

        [Required]
        [StringLength(25)]
        public string PostCode { get; set; }

        [MaxLength(50)] 
        public string City { get; set; }

        public override User ToDomainModel()
        {
            return new User
            {
                FullName = FullName,
                Email = Email,
                Phone = Phone,
                DateOfBirth = DateOfBirth,
                Address = Address,
                City = City,
                PostCode = PostCode,
                Country = Country,
            };
        }
    }
}