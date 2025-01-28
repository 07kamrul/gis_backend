namespace gis_backend.Models;

public class UserLocation
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public bool IsSharingLocation { get; set; }
}