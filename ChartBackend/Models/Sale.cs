using System;
using System.Collections.Generic;

#nullable disable

namespace ChartBackend.Models
{
    public partial class Sale
    {
        public int Id { get; set; }
        public int? PersonelId { get; set; }
        public decimal? Price { get; set; }
    }
}
