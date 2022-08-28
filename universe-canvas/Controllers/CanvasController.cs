using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using universe_canvas.Hubs;
using universe_canvas.Services;

namespace universe_canvas.Controllers
{
    [ApiController]
    public class CanvasController : ControllerBase
    {
        private readonly IHubContext<CanvasHub> _hub;
        private readonly TimerService _timer;
        
        public CanvasController(IHubContext<CanvasHub> hub, TimerService timer)
        {
            _hub = hub;
            _timer = timer;
        }

        [HttpPost]
        [Route("api/[controller]/startTimer")]
        public IActionResult StartTimer()
        {
            _timer.AddTimer(60000, () => _hub.Clients.All.SendAsync("TransferCompleteCanvas", CanvasHub.Canvas));
            _timer.AddTimer(500, () =>
            {
                _hub.Clients.All.SendAsync("TransferCanvasChanges", CanvasHub.CanvasChanges);
                CanvasHub.ClearChanges();
            });
            return NoContent();
        }

        [HttpPost]
        [Route("api/[controller]/stopTimer")]
        public IActionResult StopTimer()
        {
            _timer.StopAllTimers();
            return NoContent();
        }
    }
}