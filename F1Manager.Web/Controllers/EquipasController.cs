using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;

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
        public async Task<ActionResult<IEnumerable<Equipa>>> GetEquipas()
        {
            // Nota: Em cenários reais, usaríamos DTOs para evitar ciclos de referência com Pilotos
            return await _context.Equipas.ToListAsync();
        }

        // GET: api/Equipas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Equipa>> GetEquipa(int id)
        {
            var equipa = await _context.Equipas.FindAsync(id);

            if (equipa == null)
            {
                return NotFound();
            }

            return equipa;
        }

        // PUT: api/Equipas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipa(int id, Equipa equipa)
        {
            if (id != equipa.Id)
            {
                return BadRequest();
            }

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
        public async Task<ActionResult<Equipa>> PostEquipa(Equipa equipa)
        {
            _context.Equipas.Add(equipa);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEquipa", new { id = equipa.Id }, equipa);
        }

        // DELETE: api/Equipas/5
        [HttpDelete("{id}")]
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