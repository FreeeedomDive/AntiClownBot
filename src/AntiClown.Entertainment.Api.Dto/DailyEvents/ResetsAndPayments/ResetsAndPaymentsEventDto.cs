namespace AntiClown.Entertainment.Api.Dto.DailyEvents.ResetsAndPayments;

public class ResetsAndPaymentsEventDto : DailyEventBaseDto
{
    public override DailyEventTypeDto Type => DailyEventTypeDto.PaymentsAndResets;
}