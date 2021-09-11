using Microsoft.AspNetCore.Mvc;

namespace AntiClownBotApi.Database.DBControllers
{
    [ApiController]
    [Route("/api/users/{userId}/items")]
    public class ItemsController
    {
        [HttpGet("")]
        public void AllItems(ulong userId)
        {
            
        }
    }
}