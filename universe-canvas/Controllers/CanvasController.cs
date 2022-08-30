using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using universe_canvas.Hubs;
using universe_canvas.Models;

namespace universe_canvas.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CanvasController : ControllerBase
{
    private readonly IHubContext<CanvasHub> _hub;
        
    public CanvasController(IHubContext<CanvasHub> hub)
    {
        _hub = hub;
    }

    [HttpPost]
    [Route("setSize")]
    public IActionResult SetSize(int width, int height, bool forceSmaller = false)
    {
        if (width < (forceSmaller ? 0 : CanvasHub.Canvas.Width) || height < (forceSmaller ? 0 : CanvasHub.Canvas.Height) || width > 1000 || height > 1000)
        {
            return BadRequest("Dimension out of range");
        }
        CanvasHub.Canvas.SetSize(width, height, forceSmaller);
        _hub.Clients.All.SendAsync("TransferCompleteCanvas", CanvasHub.Canvas);
        return Ok();
    }
    
    [HttpPost]
    [Route("fixSize")]
    public IActionResult FixSize(int width, int height, bool forceSmaller = false)
    {
        CanvasHub.Canvas.FixSize();
        return Ok();
    }
        
    [HttpPost]
    [Route("addColor")]
    public IActionResult AddColor(string hexCode)
    {
        if (!Regex.Match(hexCode, "^#(?:[0-9a-fA-F]{3}){1,2}$").Success)
        {
            return BadRequest("Invalid hex code");
        }
        CanvasHub.Canvas.Palette.Add(hexCode);
        _hub.Clients.All.SendAsync("TransferCompleteCanvas", CanvasHub.Canvas);
        return Ok();
    }
        
    [HttpPost]
    [Route("removeColor")]
    public IActionResult RemoveColor(int index)
    {
        if (index < 0 || index >= CanvasHub.Canvas.Palette.Count)
        {
            return BadRequest("Index out of range");
        }
        CanvasHub.Canvas.Palette.RemoveAt(index);
        _hub.Clients.All.SendAsync("TransferCompleteCanvas", CanvasHub.Canvas);
        return Ok();
    }
        
    [HttpPost]
    [Route("removeLastColor")]
    public IActionResult RemoveLastColor()
    {
        if (CanvasHub.Canvas.Palette.Count == 0)
        {
            return BadRequest("Palette empty");
        }
        CanvasHub.Canvas.Palette.RemoveAt(CanvasHub.Canvas.Palette.Count-1);
        _hub.Clients.All.SendAsync("TransferCompleteCanvas", CanvasHub.Canvas);
        return Ok();
    }
        
}