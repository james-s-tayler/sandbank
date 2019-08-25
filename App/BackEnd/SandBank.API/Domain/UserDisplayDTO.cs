using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    //I don't love this, but it's slightly less evil than AutoMapper
    public class UserDisplayDTO
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

        public UserDisplayDTO(User user)
        {
            Id = user.Id;
            FullName = user.FullName;
            Email = user.Email;
            Phone = user.Phone;
            DateOfBirth = user.DateOfBirth;
            Address = user.Address;
            City = user.City;
        }
    }
}