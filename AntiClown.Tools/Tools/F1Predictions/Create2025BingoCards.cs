using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo;

namespace AntiClown.Tools.Tools.F1Predictions;

public class Create2025BingoCards(IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient, ILogger<Create2025BingoCards> logger) : ToolBase(logger)
{
    protected override async Task RunAsync()
    {
        const int season = 2025;
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Победа гонщика команды не из четверки лидеров",
            F1BingoCardProbabilityDto.Medium,
            1,
            "Победил гонщик не из Red Bull, McLaren, Ferrari или Mercedes"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Подиум гонщика команды не из четверки лидеров",
            F1BingoCardProbabilityDto.High,
            5,
            "На подиуме гонщик не из Red Bull, McLaren, Ferrari или Mercedes"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Хэмильтон 8-кратный чемпион",
            F1BingoCardProbabilityDto.Low
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Red Bull посреди сезона меняют гонщика в команде",
            F1BingoCardProbabilityDto.Medium
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Двойной сход Ferrari",
            F1BingoCardProbabilityDto.High,
            1,
            "Учитывается только гонка, спринт не считается"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Гонка закончилась не по кругам, а по таймеру",
            F1BingoCardProbabilityDto.Low
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Гонщик по результатам гонки прорвался на 10+ позиций",
            F1BingoCardProbabilityDto.High,
            3
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Обгон на последнем круге за победу",
            F1BingoCardProbabilityDto.Medium
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Авария на 1 круге",
            F1BingoCardProbabilityDto.High,
            5
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Дождевая гонка",
            F1BingoCardProbabilityDto.Medium,
            3,
            "Засчитывается при появлении промежуточных или дождевых шин хотя бы у одного из гонщиков"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Авария тиммейтов",
            F1BingoCardProbabilityDto.Low,
            1,
            "Обязателен сход одного из них"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Красные флаги в квалификации",
            F1BingoCardProbabilityDto.Medium,
            3,
            "2 красных флага в одной квалификации (даже если в разных сегментах) не дают 2 очка, спринт-квалификации учитываются"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Гонщик объявляет о завершении карьеры",
            F1BingoCardProbabilityDto.Low,
            1,
            "Hulkenberg? Alonso? Stroll? Verstappen???"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "5 побед Макса подряд",
            F1BingoCardProbabilityDto.Medium,
            5
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Гонка с 0 DNF",
            F1BingoCardProbabilityDto.High,
            3
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "2 редфлага в одной гонке",
            F1BingoCardProbabilityDto.High
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Новичок на подиуме",
            F1BingoCardProbabilityDto.Medium,
            3,
            "Новички сезона - Antonelli, Doohan, Bearman, Hadjar, Bortoleto"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Дисквалификация гонщика по причине нехватки топлива в баке",
            F1BingoCardProbabilityDto.Low
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Штраф Stop and Go",
            F1BingoCardProbabilityDto.Low
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Хотя бы у 2 гонщиков по результатам сезона затраты на аварии менее 400.000 евро",
            F1BingoCardProbabilityDto.Low
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Сайнц в гравии",
            F1BingoCardProbabilityDto.Medium,
            3,
            "Пошел по стопам отца в ралли (считаем только в квалах и гонках)"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Серая зона в правилах",
            F1BingoCardProbabilityDto.Low,
            1,
            "По ходу сезона одна из команд нашла серую зону в правилах и применила какую-то модификацию болида, и ее запретили"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Каждая команда наберет очки",
            F1BingoCardProbabilityDto.Medium
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Интрига до последней гонки",
            F1BingoCardProbabilityDto.Medium,
            1,
            "Чемпион в КК или ЛЗ не определен до последней гонки"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Клоунский пит-стоп",
            F1BingoCardProbabilityDto.High,
            1,
            "Пит-стоп привел к сходу болида либо к потере 20+ секунд, решение о достаточности клоунства пит-стопа принимается коллегией дискорда"
        );
    }
}