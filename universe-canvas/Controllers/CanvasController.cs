using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using universe_canvas.Hubs;
using universe_canvas.TimerFeatures;

namespace universe_canvas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CanvasController : ControllerBase
    {
        private readonly IHubContext<CanvasHub> _hub;
        private readonly TimerManager _timer;
        
        public CanvasController(IHubContext<CanvasHub> hub, TimerManager timer)
        {
            _hub = hub;
            _timer = timer;
        }

        [HttpPost]
        public IActionResult StartTimer()
        {
            if (!_timer.IsTimerStarted)
                _timer.PrepareTimer(500, () => _hub.Clients.All.SendAsync("TransferCompleteCanvas", CanvasHub.Canvas));
            return NoContent();
        }

        [HttpPost]
        public IActionResult StopTimer()
        {
            _timer.StopTimer();
            return NoContent();
        }
    }
}