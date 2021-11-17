using System;
using System.Collections.Generic;
using AntiClownBotApi.Database;
using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;
using AntiClownBotApi.Models.Items;
using Microsoft.AspNetCore.Mvc;

namespace AntiClownBotApi.Controllers
{
    [ApiController]
    [Route("/api/globalState")]
    public class GlobalStateController
    {
        private UserRepository UserRepository { get; }
        private DatabaseContext Database { get; }

        public GlobalStateController(DatabaseContext database, UserRepository userRepository)
        {
            Database = database;
            UserRepository = userRepository;
        }
        
        [HttpGet, Route("ping")]
        public string Ping()
        {
            return "OK";
        }
        
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

        [HttpGet, Route("forSomeSituations")]
        public void ForSomeSituations()
        {
            
        }
    }
}