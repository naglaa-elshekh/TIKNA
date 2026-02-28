using System;
using System.ComponentModel.DataAnnotations;

namespace TIKNA.Models
{
    public class Rental
    {
        public int RentalId { get; set; }

        // FK → المنتج المستأجر
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // FK → المستأجر
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Payment for the rental
        public Payment Payment { get; set; }
    }
}