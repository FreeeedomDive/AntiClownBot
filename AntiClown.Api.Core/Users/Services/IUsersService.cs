﻿using AntiClown.Api.Core.Users.Domain;

namespace AntiClown.Api.Core.Users.Services;

public interface IUsersService
{
    Task<User[]> ReadAllAsync();
    Task<User> ReadAsync(Guid id);
    Task<User[]> FindAsync(UserFilter filter);
    Task<User?> FindByIntegrationIdAsync(UserIntegrationFilter filter);
    Task CreateOrUpdateCustomIntegration(Guid userId, string integrationName, string integrationUserId);
    Task BindTelegramUserIdAsync(Guid id, long telegramUserId);
}