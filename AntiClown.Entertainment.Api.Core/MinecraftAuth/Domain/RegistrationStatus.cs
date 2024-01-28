namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Domain;

public enum RegistrationStatus
{
    // ReSharper disable once InconsistentNaming
    FailedUpdate_NicknameOwnedByOtherUser,
    // ReSharper disable once InconsistentNaming
    FailedCreate_NicknameOwnedByOtherUser,
    SuccessCreate,
    SuccessUpdate,
}