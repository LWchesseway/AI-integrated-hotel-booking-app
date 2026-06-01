namespace DoAn.HotelParking.Infrastructure.Data.Seeding;

public class SeedDataOptions
{
    public bool Enabled { get; set; } = true;
    public bool AllowInProduction { get; set; } = false;
    public bool SkipIfHasData { get; set; } = true;
    public int RandomSeed { get; set; } = 20260527;
    public int OwnerCount { get; set; } = 15;
    public int CustomerCount { get; set; } = 80;
    public int HotelsPerOwner { get; set; } = 3;
    public int RoomsPerHotel { get; set; } = 8;
    public int TimeSlotsPerRoom { get; set; } = 6;
    public int BookingsPerRoom { get; set; } = 3;
    public int FavoriteHotelsPerCustomer { get; set; } = 4;
    public int ReviewPercent { get; set; } = 40;
    public decimal DepositRate { get; set; } = 0.3m;
}
