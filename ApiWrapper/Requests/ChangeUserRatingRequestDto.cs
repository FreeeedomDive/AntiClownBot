namespace ApiWrapper.Requests
{
    public class ChangeUserRatingRequestDto: BaseRequestDto
    {
        public int RatingDiff { get; set; }
        public string Reason { get; set; }
    }
}