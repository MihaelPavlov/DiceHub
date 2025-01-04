namespace DH.Statistics.Domain.Models.Queries;

public class GetReservationChartData
{
    public ReservationStats GameReservationStats { get; set; } = null!;
    public ReservationStats TableReservationStats { get; set; } = null!;
}

public class ReservationStats
{
    public int Completed { get; set; }
    public int Cancelled { get; set; }
}
