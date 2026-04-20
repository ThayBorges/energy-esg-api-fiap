using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnergyESG.Infrastructure.Data;
using EnergyESG.Application.ViewModels;

namespace EnergyESG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RelatoriosController : ControllerBase
{
    private readonly EnergyContext _context;

    public RelatoriosController(EnergyContext context)
    {
        _context = context;
    }

    [HttpGet("energia")]
    public async Task<IActionResult> Energia(
        [FromQuery] Guid unidadeId,
        [FromQuery] DateTime de,
        [FromQuery] DateTime ate)
    {
        if (de >= ate)
            return BadRequest("Data inicial deve ser anterior à data final");

        var dados = await _context.Consumos
            .AsNoTracking()
            .Where(x => x.UnidadeId == unidadeId && x.DataHora >= de && x.DataHora <= ate)
            .GroupBy(x => x.UnidadeId)
            .Select(g => new RelatorioEnergiaVm
            {
                UnidadeId = g.Key,
                TotalKwh = g.Sum(x => x.Kwh),
                Picos = g.Count(x => x.UltrapassouLimite),
                MediaKwh = g.Average(x => x.Kwh),
                DataInicio = de,
                DataFim = ate
            })
            .FirstOrDefaultAsync();

        if (dados == null)
        {
            // Verifica se a unidade existe
            var unidadeExiste = await _context.Unidades
                .AnyAsync(u => u.Id == unidadeId);

            if (!unidadeExiste)
                return NotFound("Unidade não encontrada");

            return Ok(new RelatorioEnergiaVm
            {
                UnidadeId = unidadeId,
                TotalKwh = 0,
                Picos = 0,
                MediaKwh = 0,
                DataInicio = de,
                DataFim = ate
            });
        }

        return Ok(dados);
    }

    [HttpGet("consumo-por-periodo")]
    public async Task<IActionResult> ConsumoPorPeriodo(
        [FromQuery] Guid unidadeId,
        [FromQuery] DateTime de,
        [FromQuery] DateTime ate,
        [FromQuery] string agrupamento = "dia") // dia, semana, mes
    {
        if (de >= ate)
            return BadRequest("Data inicial deve ser anterior à data final");

        var consumos = await _context.Consumos
            .AsNoTracking()
            .Where(x => x.UnidadeId == unidadeId && x.DataHora >= de && x.DataHora <= ate)
            .OrderBy(x => x.DataHora)
            .Select(x => new
            {
                x.DataHora,
                x.Kwh,
                x.UltrapassouLimite
            })
            .ToListAsync();

        var resultado = agrupamento.ToLower() switch
        {
            "dia" => consumos.GroupBy(x => x.DataHora.Date)
                .Select(g => new
                {
                    Periodo = g.Key.ToString("yyyy-MM-dd"),
                    TotalKwh = g.Sum(x => x.Kwh),
                    MediaKwh = g.Average(x => x.Kwh),
                    Picos = g.Count(x => x.UltrapassouLimite)
                }),
            "semana" => consumos.GroupBy(x => new
            {
                Ano = x.DataHora.Year,
                Semana = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    x.DataHora, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday)
            })
                .Select(g => new
                {
                    Periodo = $"Semana {g.Key.Semana}/{g.Key.Ano}",
                    TotalKwh = g.Sum(x => x.Kwh),
                    MediaKwh = g.Average(x => x.Kwh),
                    Picos = g.Count(x => x.UltrapassouLimite)
                }),
            "mes" => consumos.GroupBy(x => new { x.DataHora.Year, x.DataHora.Month })
                .Select(g => new
                {
                    Periodo = $"{g.Key.Year}-{g.Key.Month:D2}",
                    TotalKwh = g.Sum(x => x.Kwh),
                    MediaKwh = g.Average(x => x.Kwh),
                    Picos = g.Count(x => x.UltrapassouLimite)
                }),
            _ => consumos.GroupBy(x => x.DataHora.Date)
                .Select(g => new
                {
                    Periodo = g.Key.ToString("yyyy-MM-dd"),
                    TotalKwh = g.Sum(x => x.Kwh),
                    MediaKwh = g.Average(x => x.Kwh),
                    Picos = g.Count(x => x.UltrapassouLimite)
                })
        };

        return Ok(resultado);
    }
}


