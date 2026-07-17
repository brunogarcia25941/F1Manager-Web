using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace F1Manager.Web.Pages.Equipas
{
    [Authorize(Roles = "Equipa")]
    public class PerfilModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public PerfilModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        [BindProperty]
        public Equipa Equipa { get; set; } = default!;

        [BindProperty]
        public IFormFile? LogotipoUpload { get; set; }

        public List<Piloto> Pilotos { get; set; } = new();
        public bool TemEquipaAssociada { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            var equipa = await _context.Equipas
                .Include(e => e.Pilotos)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (equipa == null)
            {
                TemEquipaAssociada = false;
                return Page();
            }

            TemEquipaAssociada = true;
            Equipa = equipa;
            Pilotos = equipa.Pilotos.ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _userManager.GetUserId(User);

            var equipaDb = await _context.Equipas
                .Include(e => e.Pilotos)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (equipaDb == null)
            {
                return NotFound("Equipa não encontrada.");
            }

            // A equipa edita estes campos
            equipaDb.Nome = Equipa.Nome;
            equipaDb.FabricanteMotor = Equipa.FabricanteMotor;
            equipaDb.Pais = Equipa.Pais;
            equipaDb.ChefeEquipa = Equipa.ChefeEquipa;
            equipaDb.AnoFundacao = Equipa.AnoFundacao;
            equipaDb.Historia = Equipa.Historia;

            // Remove propriedades não submetidas da validação do Model
            ModelState.Remove("LogotipoUpload");

            if (!ModelState.IsValid)
            {
                Equipa.Logotipo = equipaDb.Logotipo;
                Pilotos = equipaDb.Pilotos.ToList();
                TemEquipaAssociada = true;
                return Page();
            }

            // Lógica de Upload de Logótipo
            if (LogotipoUpload != null)
            {
                // Validações básicas
                if (LogotipoUpload.Length > 2 * 1024 * 1024) // 2MB
                {
                    ModelState.AddModelError("LogotipoUpload", "O ficheiro não pode exceder 2MB.");
                    Equipa.Logotipo = equipaDb.Logotipo;
                    Pilotos = equipaDb.Pilotos.ToList();
                    TemEquipaAssociada = true;
                    return Page();
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(LogotipoUpload.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("LogotipoUpload", "Formato de imagem não permitido.");
                    Equipa.Logotipo = equipaDb.Logotipo;
                    Pilotos = equipaDb.Pilotos.ToList();
                    TemEquipaAssociada = true;
                    return Page();
                }

                // Converte o logótipo carregado para string Base64 e guarda diretamente na base de dados
                using (var memoryStream = new MemoryStream())
                {
                    await LogotipoUpload.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    equipaDb.Logotipo = $"data:{LogotipoUpload.ContentType};base64,{Convert.ToBase64String(fileBytes)}";
                }
            }

            _context.Equipas.Update(equipaDb);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
