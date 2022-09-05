using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using universe_canvas.Hubs;
using universe_canvas.Models;
using universe_canvas.Services;

namespace universe_canvas.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CanvasController : ControllerBase
{
    private readonly IHubContext<CanvasHub> _hub;
    private readonly ICanvasService _canvasService;

    public CanvasController(IHubContext<CanvasHub> hub, ICanvasService canvasService)
    {
        _hub = hub;
        _canvasService = canvasService;
    }

    [HttpPost]
    [Route("setSize")]
    public IActionResult SetSize(int width, int height, bool forceSmaller = false)
    {
        if (width < (forceSmaller ? 0 : _canvasService.Canvas.Width) || height < (forceSmaller ? 0 : _canvasService.Canvas.Height) || width > 1000 || height > 1000)
        {
            return BadRequest("Dimension out of range");
        }
        _canvasService.Canvas.SetSize(width, height, forceSmaller);
        _hub.Clients.All.SendAsync("TransferCompleteCanvas", _canvasService.Canvas);
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
        _canvasService.Canvas.Palette.Add(hexCode);
        _hub.Clients.All.SendAsync("TransferCompleteCanvas", _canvasService.Canvas);
        return Ok();
    }
        
    [HttpPost]
    [Route("removeColor")]
    public IActionResult RemoveColor(int index)
    {
        if (index < 0 || index >= _canvasService.Canvas.Palette.Count)
        {
            return BadRequest("Index out of range");
        }
        _canvasService.Canvas.RemoveColor(index);
        _hub.Clients.All.SendAsync("TransferCompleteCanvas", _canvasService.Canvas);
        return Ok();
    }
        
    [HttpPost]
    [Route("removeLastColor")]
    public IActionResult RemoveLastColor()
    {
        if (_canvasService.Canvas.Palette.Count == 0)
        {
            return BadRequest("Palette empty");
        }
        _canvasService.Canvas.RemoveColor(_canvasService.Canvas.Palette.Count-1);
        _hub.Clients.All.SendAsync("TransferCompleteCanvas", _canvasService.Canvas);
        return Ok();
    }
    
    [HttpPost]
    [Route("updatePalette")]
    public IActionResult UpdatePalette(PaletteChangeDto newPalette)
    {
        _canvasService.Canvas.UpdatePalette(newPalette);
        _hub.Clients.All.SendAsync("TransferCompleteCanvas", _canvasService.Canvas);
        return Ok();
    }
}