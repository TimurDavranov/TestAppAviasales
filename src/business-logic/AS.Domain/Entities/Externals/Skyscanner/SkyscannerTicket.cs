﻿using AS.Core.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Domain.Entities.Externals.Skyscanner
{
    [Table("skyscanner_tickets", Schema = "externals")]
    public class SkyscannerTicket : BaseEntity<Guid>
    {
        public string CountryCode { get; set; }
        public Guid CityId { get; set; }
        public string AirportCode { get; set; }
        public DateTime DepartureDate { get; set; }
        public decimal Price { get; set; }
        public int NumberOfSeats { get; set; }
        public Guid AviaCompanyId { get; set; }
        public ICollection<SkyscannerTicketDestination> Destinations { get; set; }
    }
}