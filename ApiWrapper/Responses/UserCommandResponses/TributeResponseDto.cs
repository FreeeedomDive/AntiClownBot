﻿using System;
using System.Collections.Generic;
using ApiWrapper.Models.Items;

namespace ApiWrapper.Responses.UserCommandResponses
{
    public class TributeResponseDto: BaseResponseDto
    {
        public int TributeQuality { get; set; }
        public bool IsTributeAutomatic { get; set; }
        public bool IsNextTributeAutomatic { get; set; }
        public Dictionary<Guid, int> CooldownModifiers { get; set; }
        public bool IsCommunismActive { get; set; }
        public ulong SharedCommunistUserId { get; set; }
        public TributeResult Result { get; set; }
    }

    public enum TributeResult
    {
        Success,
        CooldownHasNotPassed,
        AutoTributeWasCancelledByEarlierTribute
    }
}