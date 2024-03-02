using AntiClown.DiscordBot.Releases.Domain;

namespace AntiClown.DiscordBot.Releases.Repositories;

public class CurrentReleaseProvider : ICurrentReleaseProvider
{
    public ReleaseVersion GetCurrentRelease()
    {
        return new ReleaseVersion
        {
            Major = 3,
            Minor = 4,
            Patch = 0,
            Description =
"""
Переработана система предсказаний на гонку
Теперь предсказания вносятся через новую веб-страницу бота 
Чтобы на нее попасть, выполни команду /web
Что нового:
1. предсказаний стало больше
2. пока что выбирается первая активная гонка, но скоро можно будет выбирать, на какую гонку делается предсказание (в случае спринтов)
3. после сохранения предсказания оно отобразится на странице после перезагрузки, и его можно будет легко изменить
4. в будущем добавятся оповещения в чат формулы-1 о появлении новых предсказаний и внесённых результатах
""",
            CreatedAt = DateTime.UtcNow,
        };
    }
}