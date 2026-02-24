using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo;

namespace AntiClown.Tools.Tools.F1Predictions;

public class Create2026BingoCards(
    IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
    ILogger<Create2026BingoCards> logger
) : ToolBase(logger)
{
    protected override async Task RunAsync()
    {
        const int season = 2026;
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Победа гонщика команды не из четверки лидеров",
            F1BingoCardProbabilityDto.Low,
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
            "Гонщик по результатам гонки прорвался на 10+ позиций",
            F1BingoCardProbabilityDto.High,
            3
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
            "Гонка с 0 DNF",
            F1BingoCardProbabilityDto.High,
            3
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
            "Доминация в чемпионате",
            F1BingoCardProbabilityDto.High,
            1,
            "По результатам сезона чемпион обгоняет 2 место на 100+ очков, "
            + "либо чемпионская команда имеет в 2 раза больше очков команды на 2 месте"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Монако",
            F1BingoCardProbabilityDto.High,
            1,
            "На гран-при Монако было меньше 3 обгонов за позицию"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Рекурсия штрафов",
            F1BingoCardProbabilityDto.High,
            1,
            "Пилот получил штраф за то, что неправильно отбыл предыдущий штраф"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Пит-стоп не нужен",
            F1BingoCardProbabilityDto.Low,
            1,
            "Пилот проедет всю гонку на одном комплекте и сменит шины на последнем круге"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Бездарь",
            F1BingoCardProbabilityDto.High,
            1,
            "Пилот не наберет ни одного очка за весь сезон"
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "5+ DNF в гонке",
            F1BingoCardProbabilityDto.Medium
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Хотя бы у 2 гонщиков по результатам сезона затраты на аварии менее 400.000 евро",
            F1BingoCardProbabilityDto.Low
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Обгон на последнем круге за победу",
            F1BingoCardProbabilityDto.Medium
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Новый чемпион",
            F1BingoCardProbabilityDto.Medium
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
            "2 редфлага в одной гонке",
            F1BingoCardProbabilityDto.High
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Дисквалификация гонщика по причине нехватки топлива в баке",
            F1BingoCardProbabilityDto.Low
        );
        await antiClownEntertainmentApiClient.F1Bingo.CreateBingoCard(
            season,
            "Победа гонщика на домашней трассе",
            F1BingoCardProbabilityDto.Medium
        );
    }
}