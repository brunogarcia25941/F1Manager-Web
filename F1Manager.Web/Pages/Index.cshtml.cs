using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;

namespace F1Manager.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Guarda os favoritos mapeados do utilizador atual
        public Favorito? FavoritosUtilizador { get; set; }

        public async Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                // Carrega os dados do piloto (com a respetiva equipa) e a equipa favorita
                FavoritosUtilizador = await _context.Favoritos
                    .Include(f => f.Piloto)
                        .ThenInclude(p => p.Equipa)
                    .Include(f => f.Equipa)
                    .FirstOrDefaultAsync(f => f.UserId == userId);
            }
        }
    }
}