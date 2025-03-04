﻿using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Core.Dto.Exceptions;

public class EntityNotFoundException : NotFoundException
{
    public EntityNotFoundException(string entityName) : base($"Entity with name {entityName} does not exist")
    {
    }

    public EntityNotFoundException(Guid id) : base($"Entity with id {id} does not exist")
    {
    }
}