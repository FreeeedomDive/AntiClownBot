﻿namespace AntiClown.Entertainment.Api.Core.Parties.Domain;

public class CreateParty
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int MaxPlayers { get; set; }
    public Guid CreatorId { get; set; }
    public ulong RoleId { get; set; }
    public bool AuthorAutoJoin { get; set; }
}