using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using Core;

namespace Domain.User
{
    //I don't love this, but it's slightly less evil than AutoMapper
    public class UserViewModel
    {
        public int Id { get; }
        
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; }

        [MaxLength(25)] 
        public string Phone { get; }
        public DateTime DateOfBirth { get; }

        [MaxLength(250)] 
        public string Address { get; }

        [MaxLength(50)] 
        public string City { get; }
        
        [Required]
        [StringLength(50)]
        public string Country { get; set; }

        [Required]
        [StringLength(25)]
        public string PostCode { get; set; }

        public UserViewModel(User user)
        {
            Id = user.Id;
            FullName = user.FullName;
            Email = user.Email;
            Phone = user.Phone;
            DateOfBirth = user.DateOfBirth;
            Address = user.Address;
            City = user.City;
            PostCode = user.PostCode;
            Country = user.Country;
        }
    }
}