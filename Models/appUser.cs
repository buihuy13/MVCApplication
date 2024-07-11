using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MVCTest.Models
{
    public class AppUser : IdentityUser 
    {
          [StringLength(400)]  
          public string? HomeAddress { get; set; }
          public DateTime? DateOfBirth { get; set; }
    }
}
