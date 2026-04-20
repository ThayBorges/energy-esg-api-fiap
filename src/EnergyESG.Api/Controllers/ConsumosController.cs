using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnergyESG.Infrastructure.Data;
using EnergyESG.Application.DTOs;
using EnergyESG.Application.ViewModels;
using EnergyESG.Domain.Entities;
using FluentValidation;

namespace EnergyESG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsumosController : ControllerBase
{
    private readonly EnergyContext _context;
    private readonly IValidator<CreateConsumoDto> _validator;

    public ConsumosController(EnergyContext context, IValidator<CreateConsumoDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 20,
        [FromQuery] Guid? unidadeId = null,
        [FromQuery] DateTime? de = null,
        [FromQuery] DateTime? ate = null)
    {
        if (pagina < 1) pagina = 1;
        if (tamanho < 1 || tamanho > 100) tamanho = 20;

        var query = _context.Consumos.AsNoTracking();

        if (unidadeId.HasValue)
            query = query.Where(x => x.UnidadeId == unidadeId.Value);

        if (de.HasValue)
            query = query.Where(x => x.DataHora >= de.Value);

        if (ate.HasValue)
            query = query.Where(x => x.DataHora <= ate.Value);

        var total = await query.CountAsync();

        var itens = await query
            .OrderByDescending(x => x.DataHora)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .Select(x => new ConsumoVm
            {
                Id = x.Id,
                UnidadeId = x.UnidadeId,
                DataHora = x.DataHora,
                Kwh = x.Kwh,
                UltrapassouLimite = x.UltrapassouLimite
            })
            .ToListAsync();

        Response.Headers.Append("X-Total-Count", total.ToString());
        Response.Headers.Append("X-Page", pagina.ToString());
        Response.Headers.Append("X-Page-Size", tamanho.ToString());

        return Ok(new PagedResultVm<ConsumoVm>
        {
            Items = itens,
            TotalCount = total,
            Page = pagina,
            PageSize = tamanho
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var consumo = await _context.Consumos
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ConsumoVm
            {
                Id = x.Id,
                UnidadeId = x.UnidadeId,
                DataHora = x.DataHora,
                Kwh = x.Kwh,
                UltrapassouLimite = x.UltrapassouLimite
            })
            .FirstOrDefaultAsync();

        if (consumo == null)
            return NotFound();

        return Ok(consumo);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateConsumoDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        // Verifica se a unidade existe
        var unidadeExiste = await _context.Unidades
            .AnyAsync(u => u.Id == dto.UnidadeId && u.Ativo);

        if (!unidadeExiste)
            return BadRequest("Unidade não encontrada ou inativa");

        // Busca regra de alerta ativa para a unidade
        var regra = await _context.RegrasAlertas
            .FirstOrDefaultAsync(r => r.UnidadeId == dto.UnidadeId && r.Ativo);

        var ultrapassouLimite = regra != null && dto.Kwh > regra.LimiteKwhHora;

        var entity = new ConsumoEnergia
        {
            Id = Guid.NewGuid(),
            UnidadeId = dto.UnidadeId,
            DataHora = dto.DataHora,
            Kwh = dto.Kwh,
            UltrapassouLimite = ultrapassouLimite
        };

        _context.Consumos.Add(entity);
        await _context.SaveChangesAsync();

        var vm = new ConsumoVm
        {
            Id = entity.Id,
            UnidadeId = entity.UnidadeId,
            DataHora = entity.DataHora,
            Kwh = entity.Kwh,
            UltrapassouLimite = entity.UltrapassouLimite
        };

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, vm);
    }
}


