using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVCTest.Models
{
    public class ContactModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("Full Name")]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Date Sent")]
        public DateTime DateSent { get; set; }
        public string? Message { get; set; }
        [StringLength(10)]
        [DisplayName("Phone Number")]
        public string? PhoneNumber { get; set; }

    }
}
