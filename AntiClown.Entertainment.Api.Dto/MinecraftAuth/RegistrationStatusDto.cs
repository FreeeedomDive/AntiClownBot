namespace AntiClown.Entertainment.Api.Dto.MinecraftAuth;

public enum RegistrationStatusDto
{
    // ReSharper disable once InconsistentNaming
    FailedUpdate_NicknameOwnedByOtherUser,
    // ReSharper disable once InconsistentNaming
    FailedCreate_NicknameOwnedByOtherUser,
    SuccessCreate,
    SuccessUpdate,
}