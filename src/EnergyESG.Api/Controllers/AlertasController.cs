using Microsoft.AspNetCore.Authorization;
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
[Authorize(Policy = "GestorEnergia")]
public class AlertasController : ControllerBase
{
    private readonly EnergyContext _context;
    private readonly IValidator<CreateRegraAlertaDto> _validator;

    public AlertasController(EnergyContext context, IValidator<CreateRegraAlertaDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid unidadeId)
    {
        var regras = await _context.RegrasAlertas
            .AsNoTracking()
            .Where(r => r.UnidadeId == unidadeId && r.Ativo)
            .Select(r => new RegraAlertaVm
            {
                Id = r.Id,
                UnidadeId = r.UnidadeId,
                LimiteKwhHora = r.LimiteKwhHora,
                Ativo = r.Ativo,
                Descricao = r.Descricao
            })
            .ToListAsync();

        return Ok(regras);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateRegraAlertaDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        // Verifica se a unidade existe
        var unidadeExiste = await _context.Unidades
            .AnyAsync(u => u.Id == dto.UnidadeId && u.Ativo);

        if (!unidadeExiste)
            return BadRequest("Unidade não encontrada ou inativa");

        // Desativa regras anteriores para a mesma unidade
        var regrasAnteriores = await _context.RegrasAlertas
            .Where(r => r.UnidadeId == dto.UnidadeId && r.Ativo)
            .ToListAsync();

        foreach (var regra in regrasAnteriores)
        {
            regra.Ativo = false;
        }

        var entity = new RegraAlerta
        {
            Id = Guid.NewGuid(),
            UnidadeId = dto.UnidadeId,
            LimiteKwhHora = dto.LimiteKwhHora,
            Ativo = true,
            Descricao = dto.Descricao
        };

        _context.RegrasAlertas.Add(entity);
        await _context.SaveChangesAsync();

        var vm = new RegraAlertaVm
        {
            Id = entity.Id,
            UnidadeId = entity.UnidadeId,
            LimiteKwhHora = entity.LimiteKwhHora,
            Ativo = entity.Ativo,
            Descricao = entity.Descricao
        };

        return CreatedAtAction(nameof(Get), new { unidadeId = entity.UnidadeId }, vm);
    }
}


