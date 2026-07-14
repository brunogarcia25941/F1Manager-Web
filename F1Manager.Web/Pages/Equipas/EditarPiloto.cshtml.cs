using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace F1Manager.Web.Pages.Equipas
{
    [Authorize(Roles = "Equipa")]
    public class EditarPilotoModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public EditarPilotoModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        [BindProperty]
        public Piloto Piloto { get; set; } = default!;

        [BindProperty]
        public IFormFile? FotoUpload { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = _userManager.GetUserId(User);

            // Verifica se a equipa pertence ao utilizador
            var equipa = await _context.Equipas
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (equipa == null)
            {
                ErrorMessage = "Equipa não encontrada.";
                return Page();
            }

            // Verifica se o piloto pertence à equipa do utilizador
            Piloto = await _context.Pilotos
                .FirstOrDefaultAsync(p => p.Id == id && p.EquipaId == equipa.Id);

            if (Piloto == null)
            {
                return NotFound("Piloto não encontrado.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userId = _userManager.GetUserId(User);

            var equipa = await _context.Equipas
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (equipa == null)
            {
                ErrorMessage = "Equipa não encontrada.";
                return Page();
            }

            var piloto = await _context.Pilotos
                .FirstOrDefaultAsync(p => p.Id == id && p.EquipaId == equipa.Id);

            if (piloto == null)
            {
                return NotFound("Piloto não encontrado.");
            }

            // Atualiza os dados do piloto
            piloto.Nome = Piloto.Nome;
            piloto.NumeroCarro = Piloto.NumeroCarro;
            piloto.Peso = Piloto.Peso;
            piloto.Biografia = Piloto.Biografia;

            // Remove propriedades não submetidas da validação
            ModelState.Remove("FotoUpload");

            if (!ModelState.IsValid)
            {
                Piloto.FotoPerfil = piloto.FotoPerfil;
                return Page();
            }

            // Lógica de Upload de Imagem
            if (FotoUpload != null && FotoUpload.Length > 0)
            {
                // Validações básicas
                if (FotoUpload.Length > 2 * 1024 * 1024) // 2MB
                {
                    ModelState.AddModelError("FotoUpload", "O ficheiro não pode exceder 2MB.");
                    Piloto.FotoPerfil = piloto.FotoPerfil;
                    return Page();
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(FotoUpload.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("FotoUpload", "Formato de imagem não permitido.");
                    Piloto.FotoPerfil = piloto.FotoPerfil;
                    return Page();
                }

                var folderPath = Path.Combine(_environment.WebRootPath, "uploads", "pilotos");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FotoUpload.FileName);
                var filePath = Path.Combine(folderPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await FotoUpload.CopyToAsync(stream);
                }

                piloto.FotoPerfil = $"/uploads/pilotos/{uniqueFileName}";
            }

            _context.Pilotos.Update(piloto);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Equipas/Perfil");
        }
    }
}
