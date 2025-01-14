using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? AddressExtra { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string ZipCode { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Province { get; set; }

        [Required]
        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ContactName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Reference { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
