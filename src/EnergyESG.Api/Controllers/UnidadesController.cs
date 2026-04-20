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
public class UnidadesController : ControllerBase
{
    private readonly EnergyContext _context;
    private readonly IValidator<CreateUnidadeDto> _validator;

    public UnidadesController(EnergyContext context, IValidator<CreateUnidadeDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 20,
        [FromQuery] bool? ativo = null)
    {
        if (pagina < 1) pagina = 1;
        if (tamanho < 1 || tamanho > 100) tamanho = 20;

        var query = _context.Unidades.AsNoTracking();

        if (ativo.HasValue)
            query = query.Where(u => u.Ativo == ativo.Value);

        var total = await query.CountAsync();

        var itens = await query
            .OrderBy(u => u.Nome)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .Select(u => new UnidadeVm
            {
                Id = u.Id,
                Nome = u.Nome,
                Endereco = u.Endereco,
                Tipo = u.Tipo,
                Ativo = u.Ativo
            })
            .ToListAsync();

        Response.Headers.Append("X-Total-Count", total.ToString());
        Response.Headers.Append("X-Page", pagina.ToString());
        Response.Headers.Append("X-Page-Size", tamanho.ToString());

        return Ok(new PagedResultVm<UnidadeVm>
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
        var unidade = await _context.Unidades
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UnidadeVm
            {
                Id = u.Id,
                Nome = u.Nome,
                Endereco = u.Endereco,
                Tipo = u.Tipo,
                Ativo = u.Ativo
            })
            .FirstOrDefaultAsync();

        if (unidade == null)
            return NotFound();

        return Ok(unidade);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateUnidadeDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var entity = new Unidade
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome,
            Endereco = dto.Endereco,
            Tipo = dto.Tipo,
            Ativo = true
        };

        _context.Unidades.Add(entity);
        await _context.SaveChangesAsync();

        var vm = new UnidadeVm
        {
            Id = entity.Id,
            Nome = entity.Nome,
            Endereco = entity.Endereco,
            Tipo = entity.Tipo,
            Ativo = entity.Ativo
        };

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, vm);
    }
}


