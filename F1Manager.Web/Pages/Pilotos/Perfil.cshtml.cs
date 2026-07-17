using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace F1Manager.Web.Pages.Pilotos
{
    [Authorize(Roles = "Piloto")]
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
        public Piloto Piloto { get; set; } = default!;

        [BindProperty]
        public IFormFile? FotoUpload { get; set; }

        public bool TemPilotoAssociado { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            var piloto = await _context.Pilotos
                .Include(p => p.Equipa)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (piloto == null)
            {
                TemPilotoAssociado = false;
                return Page();
            }

            TemPilotoAssociado = true;
            Piloto = piloto;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _userManager.GetUserId(User);

            var pilotoDb = await _context.Pilotos
                .Include(p => p.Equipa)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pilotoDb == null)
            {
                return NotFound("Piloto não encontrado.");
            }

            // O piloto edita apenas estes três campos no formulário
            pilotoDb.Biografia = Piloto.Biografia;
            pilotoDb.Peso = Piloto.Peso;

            // Remove propriedades não submetidas da validação do Model (evita falhas de validação em campos que o piloto não edita)
            ModelState.Remove("Piloto.Nome");
            ModelState.Remove("Piloto.NumeroCarro");
            ModelState.Remove("Piloto.Equipa");
            ModelState.Remove("FotoUpload");

            if (!ModelState.IsValid)
            {
                // Repopula os dados de leitura a partir da base de dados para que a página renderize sem NullReferenceException
                Piloto.Nome = pilotoDb.Nome;
                Piloto.NumeroCarro = pilotoDb.NumeroCarro;
                Piloto.Equipa = pilotoDb.Equipa;
                Piloto.FotoPerfil = pilotoDb.FotoPerfil;
                TemPilotoAssociado = true;
                return Page();
            }

            // Lógica de Upload de Imagem de Perfil
            if (FotoUpload != null)
            {
                // Converte a imagem carregada para uma string Base64 e guarda diretamente na base de dados
                using (var memoryStream = new MemoryStream())
                {
                    await FotoUpload.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    pilotoDb.FotoPerfil = $"data:{FotoUpload.ContentType};base64,{Convert.ToBase64String(fileBytes)}";
                }
            }

            // Evita a edição acidental do nome e da equipa no DbContext
            _context.Entry(pilotoDb).Property(x => x.Nome).IsModified = false;
            _context.Entry(pilotoDb).Property(x => x.EquipaId).IsModified = false;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Perfil");
        }
    }
}