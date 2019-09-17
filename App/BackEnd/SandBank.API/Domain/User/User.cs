using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core;

namespace Domain.User
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(25)] public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }

        [MaxLength(250)] public string Address { get; set; }

        [MaxLength(50)] public string City { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Country { get; set; }

        [Required]
        [StringLength(25)]
        public string PostCode { get; set; }

        public List<Account.Account> Accounts { get; set; } = new List<Account.Account>();

        [Editable(false)]
        [Required]
        public DateTime CreatedOn { get; set; }

    }
}