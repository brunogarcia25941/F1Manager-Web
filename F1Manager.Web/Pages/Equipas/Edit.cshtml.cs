using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace F1Manager.Web.Pages.Equipas
{
    [Authorize(Roles = "Administrador")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public EditModel(ApplicationDbContext context) => _context = context;

        // Propriedade para o upload do logótipo no formulário
        [BindProperty]
        public IFormFile? LogoUpload { get; set; }

        [BindProperty]
        public Equipa Equipa { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Equipa = await _context.Equipas.FindAsync(id);
            if (Equipa == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove propriedades de navegação e uploads da validação base
            ModelState.Remove("LogoUpload");
            ModelState.Remove("Equipa.Pilotos");

            if (!ModelState.IsValid) return Page();

            // Processamento do upload do logótipo da equipa
            if (LogoUpload != null)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(LogoUpload.FileName);
                var filePath = Path.Combine(folderPath, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await LogoUpload.CopyToAsync(fileStream);
                }

                Equipa.Logotipo = "/uploads/" + uniqueFileName;
            }
            else
            {
                // Mantém o logótipo atual se não foi enviado nenhum novo ficheiro
                var equipaExistente = await _context.Equipas.AsNoTracking().FirstOrDefaultAsync(e => e.Id == Equipa.Id);
                if (equipaExistente != null)
                {
                    Equipa.Logotipo = equipaExistente.Logotipo;
                }
            }

            // Marca o objeto como modificado para o EF fazer o Update
            _context.Attach(Equipa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}