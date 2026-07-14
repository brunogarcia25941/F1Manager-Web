using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;

namespace F1Manager.Web.Pages.Admin.Gerenciar
{
    [Authorize(Roles = "Administrador")]
    public class EquipasModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EquipasModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<EquipaComPilotos> Equipas { get; set; } = new();

        public async Task OnGetAsync()
        {
            var equipas = await _context.Equipas
                .Include(e => e.Pilotos)
                .ToListAsync();

            foreach (var equipa in equipas)
            {
                Equipas.Add(new EquipaComPilotos
                {
                    Id = equipa.Id,
                    Nome = equipa.Nome,
                    FabricanteMotor = equipa.FabricanteMotor,
                    Pais = equipa.Pais,
                    NumeroPilotos = equipa.Pilotos.Count,
                    UserId = equipa.UserId
                });
            }
        }
    }

    public class EquipaComPilotos
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string FabricanteMotor { get; set; }
        public string Pais { get; set; }
        public int NumeroPilotos { get; set; }
        public string UserId { get; set; }
    }
}
