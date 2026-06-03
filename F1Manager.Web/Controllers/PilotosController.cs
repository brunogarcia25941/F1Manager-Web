using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;

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
        public async Task<ActionResult<IEnumerable<Piloto>>> GetPilotos()
        {
            return await _context.Pilotos.ToListAsync();
        }

        // GET: api/Pilotos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Piloto>> GetPiloto(int id)
        {
            var piloto = await _context.Pilotos.FindAsync(id);

            if (piloto == null)
            {
                return NotFound();
            }

            return piloto;
        }

        // PUT: api/Pilotos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPiloto(int id, Piloto piloto)
        {
            if (id != piloto.Id)
            {
                return BadRequest();
            }

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
        public async Task<ActionResult<Piloto>> PostPiloto(Piloto piloto)
        {
            _context.Pilotos.Add(piloto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPiloto", new { id = piloto.Id }, piloto);
        }

        // DELETE: api/Pilotos/5
        [HttpDelete("{id}")]
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