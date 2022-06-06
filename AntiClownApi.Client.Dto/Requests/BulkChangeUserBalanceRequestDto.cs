namespace AntiClownApiClient.Dto.Requests
{
    public class BulkChangeUserBalanceRequestDto: BaseRequestDto
    {
        public List<ulong> Users { get; set; }
        public int RatingDiff { get; set; }
        public string Reason { get; set; }
    }
}