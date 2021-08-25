using System.Collections.Generic;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;
using Microsoft.AspNetCore.Mvc;

namespace AntiClownBotApi.Controllers
{
    [ApiController]
    [Route("/api/globalState")]
    public class GlobalStateController
    {
        [HttpGet, Route("autoTributes")]
        public List<TributeResponseDto> GetAutoTributes()
        {
            var result = GlobalState.AutomaticTributes;
            GlobalState.AutomaticTributes = new List<TributeResponseDto>();
            return result;
        }

        [HttpGet, Route("checkEvents")]
        public void CheckEvents()
        {
            // not yet implemented
        }
    }
}