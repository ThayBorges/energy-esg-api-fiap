using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnergyESG.Infrastructure.Data;
using EnergyESG.Domain.Entities;

namespace EnergyESG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "GestorEnergia")]
public class SensoresController : ControllerBase
{
    private readonly EnergyContext _context;

    public SensoresController(EnergyContext context)
    {
        _context = context;
    }

    [HttpPost("{id}/acoes/desligar")]
    public async Task<IActionResult> Desligar(Guid id)
    {
        var sensor = await _context.Sensores
            .FirstOrDefaultAsync(s => s.Id == id && s.Ativo);

        if (sensor == null)
            return NotFound("Sensor não encontrado ou inativo");

        sensor.DesligadoAutomaticamente = true;
        sensor.UltimaAcao = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            SensorId = id,
            Acao = "desligar",
            Status = "executado",
            DataHora = sensor.UltimaAcao
        });
    }

    [HttpPost("{id}/acoes/ligar")]
    public async Task<IActionResult> Ligar(Guid id)
    {
        var sensor = await _context.Sensores
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sensor == null)
            return NotFound("Sensor não encontrado");

        sensor.DesligadoAutomaticamente = false;
        sensor.UltimaAcao = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            SensorId = id,
            Acao = "ligar",
            Status = "executado",
            DataHora = sensor.UltimaAcao
        });
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? unidadeId = null)
    {
        var query = _context.Sensores.AsNoTracking();

        if (unidadeId.HasValue)
            query = query.Where(s => s.UnidadeId == unidadeId.Value);

        var sensores = await query
            .Select(s => new
            {
                s.Id,
                s.UnidadeId,
                s.Nome,
                s.Tipo,
                s.Ativo,
                s.DesligadoAutomaticamente,
                s.UltimaAcao
            })
            .ToListAsync();

        return Ok(sensores);
    }
}


