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
    public class PilotosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PilotosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Pilotos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PilotoDTO>>> GetPilotos()
        {
            return await _context.Pilotos
                .Include(p => p.Equipa)
                .Select(p => new PilotoDTO
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    NumeroCarro = p.NumeroCarro,
                    UserId = p.UserId,
                    EquipaId = p.EquipaId,
                    NomeEquipa = p.Equipa.Nome
                })
                .ToListAsync();
        }

        // GET: api/Pilotos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PilotoDTO>> GetPiloto(int id)
        {
            var piloto = await _context.Pilotos
                .Include(p => p.Equipa)
                .Select(p => new PilotoDTO
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    NumeroCarro = p.NumeroCarro,
                    UserId = p.UserId,
                    EquipaId = p.EquipaId,
                    NomeEquipa = p.Equipa.Nome
                })
                .FirstOrDefaultAsync(p => p.Id == id);

            if (piloto == null)
            {
                return NotFound();
            }

            return piloto;
        }

        // PUT: api/Pilotos/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PutPiloto(int id, PilotoDTO pilotoDto)
        {
            if (id != pilotoDto.Id)
            {
                return BadRequest();
            }

            var piloto = await _context.Pilotos.FindAsync(id);
            if (piloto == null)
            {
                return NotFound();
            }

            piloto.Nome = pilotoDto.Nome;
            piloto.NumeroCarro = pilotoDto.NumeroCarro;
            piloto.UserId = pilotoDto.UserId;
            piloto.EquipaId = pilotoDto.EquipaId;

            _context.Entry(piloto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PilotoExists(id))
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

        // POST: api/Pilotos
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<PilotoDTO>> PostPiloto(PilotoDTO pilotoDto)
        {
            var piloto = new Piloto
            {
                Nome = pilotoDto.Nome,
                NumeroCarro = pilotoDto.NumeroCarro,
                UserId = pilotoDto.UserId,
                EquipaId = pilotoDto.EquipaId
            };

            _context.Pilotos.Add(piloto);
            await _context.SaveChangesAsync();

            pilotoDto.Id = piloto.Id;

            return CreatedAtAction("GetPiloto", new { id = piloto.Id }, pilotoDto);
        }

        // DELETE: api/Pilotos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeletePiloto(int id)
        {
            var piloto = await _context.Pilotos
                .Include(p => p.Resultados)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (piloto == null)
            {
                return NotFound();
            }

            // Regra de Negócio: Não apagar pilotos com resultados registados
            if (piloto.Resultados.Any())
            {
                return BadRequest("Não é possível apagar um piloto que já tenha resultados registados em corridas.");
            }

            _context.Pilotos.Remove(piloto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PilotoExists(int id)
        {
            return _context.Pilotos.Any(e => e.Id == id);
        }
    }
}