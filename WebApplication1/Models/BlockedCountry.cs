namespace WebApplication1.Models
{
    public class BlockedCountry
    {
        public string CountryCode { get; set; } = null!;
        public string CountryName { get; set; } = null!; 
                                                       
        public DateTime? TemporalUntilUtc { get; set; }
    }

}
