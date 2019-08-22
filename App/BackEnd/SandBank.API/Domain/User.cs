using System;
using System.ComponentModel.DataAnnotations;
using Core;

namespace Domain
{
    public class User : DomainEntity<int>
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
        
        [MaxLength(50)]
        public string City { get; set; }
    }
}