namespace AntiClownApiClient.Dto.Requests
{
    public class ChangeUserRatingRequestDto: BaseRequestDto
    {
        public int RatingDiff { get; set; }
        public string Reason { get; set; }
    }
}