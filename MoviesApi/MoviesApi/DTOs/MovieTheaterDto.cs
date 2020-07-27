namespace MoviesApi.DTOs
{
    public class MovieTheaterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double DistanceInMeters { get; set; }
        public double DistanceInKms { get { return DistanceInMeters / 1000; } }
    }
}
