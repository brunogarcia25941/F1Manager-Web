using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using F1Manager.Web.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace F1Manager.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EquipasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Equipas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipaDTO>>> GetEquipas()
        {
            return await _context.Equipas
                .Select(e => new EquipaDTO
                {
                    Id = e.Id,
                    Nome = e.Nome,
                    FabricanteMotor = e.FabricanteMotor,
                    Pais = e.Pais
                })
                .ToListAsync();
        }

        // GET: api/Equipas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EquipaDTO>> GetEquipa(int id)
        {
            var equipa = await _context.Equipas
                .Select(e => new EquipaDTO
                {
                    Id = e.Id,
                    Nome = e.Nome,
                    FabricanteMotor = e.FabricanteMotor,
                    Pais = e.Pais
                })
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipa == null)
            {
                return NotFound();
            }

            return equipa;
        }

        // PUT: api/Equipas/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PutEquipa(int id, EquipaDTO equipaDto)
        {
            if (id != equipaDto.Id)
            {
                return BadRequest();
            }

            var equipa = await _context.Equipas.FindAsync(id);
            if (equipa == null)
            {
                return NotFound();
            }

            equipa.Nome = equipaDto.Nome;
            equipa.FabricanteMotor = equipaDto.FabricanteMotor;
            equipa.Pais = equipaDto.Pais;

            _context.Entry(equipa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Equipas
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<EquipaDTO>> PostEquipa(EquipaDTO equipaDto)
        {
            var equipa = new Equipa
            {
                Nome = equipaDto.Nome,
                FabricanteMotor = equipaDto.FabricanteMotor,
                Pais = equipaDto.Pais
            };

            _context.Equipas.Add(equipa);
            await _context.SaveChangesAsync();

            equipaDto.Id = equipa.Id;

            return CreatedAtAction("GetEquipa", new { id = equipa.Id }, equipaDto);
        }

        // DELETE: api/Equipas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteEquipa(int id)
        {
            var equipa = await _context.Equipas
                .Include(e => e.Pilotos)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipa == null)
            {
                return NotFound();
            }

            // Regra de Negócio: Não apagar equipas com pilotos
            if (equipa.Pilotos.Any())
            {
                return BadRequest("Não é possível apagar uma equipa que tenha pilotos associados.");
            }

            _context.Equipas.Remove(equipa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EquipaExists(int id)
        {
            return _context.Equipas.Any(e => e.Id == id);
        }
    }
}