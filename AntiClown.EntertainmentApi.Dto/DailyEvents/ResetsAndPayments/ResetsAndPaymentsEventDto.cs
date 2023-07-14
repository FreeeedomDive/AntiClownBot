namespace AntiClown.EntertainmentApi.Dto.DailyEvents.ResetsAndPayments;

public class ResetsAndPaymentsEventDto : DailyEventBaseDto
{
    public override DailyEventTypeDto Type => DailyEventTypeDto.PaymentsAndResets;
}