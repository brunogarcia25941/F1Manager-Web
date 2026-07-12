    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using F1Manager.Web.Data;
    using F1Manager.Web.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;

    namespace F1Manager.Web.Pages.Pilotos
    {
        [Authorize(Roles = "Administrador")]
        public class EditModel : PageModel
        {
            private readonly ApplicationDbContext _context;
            private readonly UserManager<IdentityUser> _userManager;

            // Injetamos o UserManager para obter acesso às contas de utilizador registadas
            public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            [BindProperty]
            public Piloto Piloto { get; set; } = default!;

            public async Task<IActionResult> OnGetAsync(int id)
            {
                Piloto = await _context.Pilotos.FindAsync(id);
                if (Piloto == null) return NotFound();

                // Preenche o dropdown de equipas
                ViewData["EquipaId"] = new SelectList(_context.Equipas, "Id", "Nome");

                // Carrega todos os utilizadores da plataforma
                var users = await _userManager.Users.ToListAsync();

                // Descobre os IDs de utilizadores que já estão associados a outros pilotos
                var associatedUserIds = await _context.Pilotos
                    .Where(p => p.Id != id && p.UserId != null)
                    .Select(p => p.UserId)
                    .ToListAsync();

                // Filtra para mostrar apenas utilizadores livres ou o utilizador atualmente associado a este piloto
                var eligibleUsers = users.Where(u => !associatedUserIds.Contains(u.Id)).ToList();
                ViewData["UserId"] = new SelectList(eligibleUsers, "Id", "Email");

                return Page();
            }

            public async Task<IActionResult> OnPostAsync()
            {
                // Remove a validação do objeto relacional complexo 'Equipa'
                ModelState.Remove("Piloto.Equipa");

                if (!ModelState.IsValid)
                {
                    ViewData["EquipaId"] = new SelectList(_context.Equipas, "Id", "Nome");

                    var users = await _userManager.Users.ToListAsync();
                    var associatedUserIds = await _context.Pilotos
                        .Where(p => p.Id != Piloto.Id && p.UserId != null)
                        .Select(p => p.UserId)
                        .ToListAsync();

                    var eligibleUsers = users.Where(u => !associatedUserIds.Contains(u.Id)).ToList();
                    ViewData["UserId"] = new SelectList(eligibleUsers, "Id", "Email");
                    return Page();
                }

                // Obtém o piloto original da BD com tracking para modificar apenas os campos necessários
                var pilotoDb = await _context.Pilotos.FirstOrDefaultAsync(p => p.Id == Piloto.Id);
                if (pilotoDb == null) return NotFound();

                var oldUserId = pilotoDb.UserId;
                var newUserId = Piloto.UserId;

                // Sincronização de Roles de Acesso (Identity)
                if (oldUserId != newUserId)
                {
                    // 1. Se foi desassociada uma conta anterior, remove a role "Piloto" dessa conta
                    if (!string.IsNullOrEmpty(oldUserId))
                    {
                        var oldUser = await _userManager.FindByIdAsync(oldUserId);
                        if (oldUser != null)
                        {
                            var outroPilotoAssociado = await _context.Pilotos.AnyAsync(p => p.Id != Piloto.Id && p.UserId == oldUserId);
                            if (!outroPilotoAssociado)
                            {
                                await _userManager.RemoveFromRoleAsync(oldUser, "Piloto");
                            }
                        }
                    }

                    // 2. Se foi associada uma nova conta, adiciona a role "Piloto" a essa conta
                    if (!string.IsNullOrEmpty(newUserId))
                    {
                        var newUser = await _userManager.FindByIdAsync(newUserId);
                        if (newUser != null)
                        {
                            if (!await _userManager.IsInRoleAsync(newUser, "Piloto"))
                            {
                                await _userManager.AddToRoleAsync(newUser, "Piloto");
                            }
                        }
                    }
                }

                // Atualiza apenas os campos controlados pelo administrador (evitando perder bio, foto ou peso do piloto)
                pilotoDb.Nome = Piloto.Nome;
                pilotoDb.NumeroCarro = Piloto.NumeroCarro;
                pilotoDb.EquipaId = Piloto.EquipaId;
                pilotoDb.UserId = newUserId;

                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
        }
    }