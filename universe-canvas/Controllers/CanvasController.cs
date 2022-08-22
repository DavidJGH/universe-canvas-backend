using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using universe_canvas.Hubs;
using universe_canvas.Models;

namespace universe_canvas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CanvasController : ControllerBase
    {
        private readonly IHubContext<CanvasHub> _hub;
        
        public CanvasController(IHubContext<CanvasHub> hub)
        {
            _hub = hub;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _hub.Clients.All.SendAsync("TransferCompleteCanvas", CanvasHub.Canvas);
            return Ok();
        }
    }
}