namespace EventManagementService.Test.ReviewEvent.V1.Models;

public class ReviewTable
{
    public int id { get; set; }
    public float rate { get; set; }
    public string reviewer_id { get; set; }
    public DateTimeOffset review_date { get; set; }
}