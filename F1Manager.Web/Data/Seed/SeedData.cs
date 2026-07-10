using Microsoft.AspNetCore.Identity;
using F1Manager.Web.Models;

namespace F1Manager.Web.Data
{
    public static class SeedData
    {
        // Este método será chamado no Program.cs
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            // Managers necessários para gerir utilizadores e permissões
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 1. Criar os Roles se não existirem
            string[] roleNames = { "Administrador", "Piloto" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Cria a role
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Criar o utilizador Administrador padrão
            var adminEmail = "admin@f1manager.pt";
            var user = await userManager.FindByEmailAsync(adminEmail);

            if (user == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true // Confirmamos o email para permitir o login imediato
                };

                // Criar o utilizador com uma password padrão (em produção, esta password deve ser alterada)
                var createPowerUser = await userManager.CreateAsync(adminUser, "Admin123!");
                if (createPowerUser.Succeeded)
                {
                    // Atribuir a role de Administrador ao novo utilizador
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                }
                else
                {
                    var errors = string.Join(", ", createPowerUser.Errors.Select(e => e.Description));
                    throw new Exception($"Erro ao criar o utilizador Administrador: {errors}");
                }
            }
            // 3. Povoamento dos dados desportivos de F1 de 2026 caso a base de dados esteja vazia
            if (!context.Campeonatos.Any())
            {
                // Criar Campeonato de F1 de 2026
                var campeonato = new Campeonato
                {
                    Nome = "Fórmula 1 - 2026",
                    Ano = 2026
                };
                context.Campeonatos.Add(campeonato);
                await context.SaveChangesAsync();

                // Criar Equipas oficiais da F1
                var ferrari = new Equipa { Nome = "Scuderia Ferrari", FabricanteMotor = "Ferrari", Pais = "Itália" };
                var redbull = new Equipa { Nome = "Oracle Red Bull Racing", FabricanteMotor = "Red Bull Ford", Pais = "Áustria" };
                var mercedes = new Equipa { Nome = "Mercedes-AMG Petronas F1 Team", FabricanteMotor = "Mercedes", Pais = "Alemanha" };
                var mclaren = new Equipa { Nome = "McLaren Mastercard F1 Team", FabricanteMotor = "Mercedes", Pais = "Reino Unido" };
                var astonMartin = new Equipa { Nome = "Aston Martin Aramco F1 Team", FabricanteMotor = "Honda", Pais = "Reino Unido" };
                var alpine = new Equipa { Nome = "BWT Alpine F1 Team", FabricanteMotor = "Mercedes", Pais = "França" };
                var williams = new Equipa { Nome = "Atlassian Williams F1 Team", FabricanteMotor = "Mercedes", Pais = "Reino Unido" };
                var racingBulls = new Equipa { Nome = "Visa Cash App Racing Bulls F1 Team", FabricanteMotor = "Red Bull Ford", Pais = "Itália" };
                var haas = new Equipa { Nome = "MoneyGram Haas F1 Team", FabricanteMotor = "Ferrari", Pais = "Estados Unidos" };
                var audi = new Equipa { Nome = "Audi F1 Team", FabricanteMotor = "Audi", Pais = "Alemanha" };
                var cadillac = new Equipa { Nome = "Cadillac Formula 1 Team", FabricanteMotor = "Ferrari", Pais = "Estados Unidos" };

                context.Equipas.AddRange(ferrari, redbull, mercedes, mclaren, astonMartin, alpine, williams, racingBulls, haas, audi, cadillac);
                await context.SaveChangesAsync();

                // Criar Pilotos oficiais e associá-los às respetivas equipas
                var pilotos = new List<Piloto>
                    {
                        // Scuderia Ferrari
                        new Piloto { Nome = "Charles Leclerc", NumeroCarro = 16, EquipaId = ferrari.Id },
                        new Piloto { Nome = "Lewis Hamilton", NumeroCarro = 44, EquipaId = ferrari.Id },

                        // Oracle Red Bull Racing
                        new Piloto { Nome = "Max Verstappen", NumeroCarro = 3, EquipaId = redbull.Id },
                        new Piloto { Nome = "Isack Hadjar", NumeroCarro = 6, EquipaId = redbull.Id },

                        // Mercedes-AMG Petronas F1 Team
                        new Piloto { Nome = "George Russell", NumeroCarro = 63, EquipaId = mercedes.Id },
                        new Piloto { Nome = "Andrea Kimi Antonelli", NumeroCarro = 12, EquipaId = mercedes.Id },

                        // McLaren Mastercard F1 Team
                        new Piloto { Nome = "Lando Norris", NumeroCarro = 1, EquipaId = mclaren.Id },
                        new Piloto { Nome = "Oscar Piastri", NumeroCarro = 81, EquipaId = mclaren.Id },

                        // Aston Martin Aramco F1 Team
                        new Piloto { Nome = "Fernando Alonso", NumeroCarro = 14, EquipaId = astonMartin.Id },
                        new Piloto { Nome = "Lance Stroll", NumeroCarro = 18, EquipaId = astonMartin.Id },

                        // BWT Alpine F1 Team
                        new Piloto { Nome = "Pierre Gasly", NumeroCarro = 10, EquipaId = alpine.Id },
                        new Piloto { Nome = "Franco Colapinto", NumeroCarro = 43, EquipaId = alpine.Id },

                        // Atlassian Williams F1 Team
                        new Piloto { Nome = "Alexander Albon", NumeroCarro = 23, EquipaId = williams.Id },
                        new Piloto { Nome = "Carlos Sainz Jr.", NumeroCarro = 55, EquipaId = williams.Id },

                        // Visa Cash App Racing Bulls F1 Team
                        new Piloto { Nome = "Liam Lawson", NumeroCarro = 30, EquipaId = racingBulls.Id },
                        new Piloto { Nome = "Arvid Lindblad", NumeroCarro = 41, EquipaId = racingBulls.Id },

                        // MoneyGram Haas F1 Team
                        new Piloto { Nome = "Esteban Ocon", NumeroCarro = 31, EquipaId = haas.Id },
                        new Piloto { Nome = "Oliver Bearman", NumeroCarro = 87, EquipaId = haas.Id },

                        // Audi F1 Team
                        new Piloto { Nome = "Nico Hülkenberg", NumeroCarro = 27, EquipaId = audi.Id },
                        new Piloto { Nome = "Gabriel Bortoleto", NumeroCarro = 5, EquipaId = audi.Id },

                        // Cadillac Formula 1 Team
                        new Piloto { Nome = "Sergio Pérez", NumeroCarro = 11, EquipaId = cadillac.Id },
                        new Piloto { Nome = "Valtteri Bottas", NumeroCarro = 77, EquipaId = cadillac.Id }
                    };

                context.Pilotos.AddRange(pilotos);
                await context.SaveChangesAsync();

                // Criar Corridas (Grandes Prémios da época 2026)
                var corridas = new List<Corrida>
                {
                    new Corrida
                    {
                        NomeGrandePremio = "GP da Austrália",
                        Circuito = "Albert Park Circuit",
                        DataHora = new DateTime(2026, 03, 15, 06, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP da China",
                        Circuito = "Circuito Internacional de Xangai",
                        DataHora = new DateTime(2026, 03, 22, 07, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP do Japão",
                        Circuito = "Circuito de Suzuka",
                        DataHora = new DateTime(2026, 04, 05, 06, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP do Bahrein",
                        Circuito = "Circuito Internacional de Sakhir",
                        DataHora = new DateTime(2026, 04, 18, 16, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP da Arábia Saudita",
                        Circuito = "Circuito de Rua de Jeddah",
                        DataHora = new DateTime(2026, 04, 25, 18, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP de Miami",
                        Circuito = "Autódromo Internacional de Miami",
                        DataHora = new DateTime(2026, 05, 03, 20, 30, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP da Emília-Romanha",
                        Circuito = "Autodromo Enzo e Dino Ferrari (Imola)",
                        DataHora = new DateTime(2026, 05, 17, 14, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP de Mónaco",
                        Circuito = "Circuito do Mónaco",
                        DataHora = new DateTime(2026, 05, 24, 14, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP de Espanha",
                        Circuito = "Circuito de Barcelona-Catalunha",
                        DataHora = new DateTime(2026, 05, 31, 14, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP do Canadá",
                        Circuito = "Circuito Gilles Villeneuve",
                        DataHora = new DateTime(2026, 06, 14, 19, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP da Áustria",
                        Circuito = "Red Bull Ring",
                        DataHora = new DateTime(2026, 06, 28, 14, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP da Grã-Bretanha",
                        Circuito = "Circuito de Silverstone",
                        DataHora = new DateTime(2026, 07, 05, 15, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP da Bélgica",
                        Circuito = "Circuito de Spa-Francorchamps",
                        DataHora = new DateTime(2026, 07, 26, 14, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP da Hungria",
                        Circuito = "Hungaroring",
                        DataHora = new DateTime(2026, 08, 02, 14, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP dos Países Baixos",
                        Circuito = "Circuito de Zandvoort",
                        DataHora = new DateTime(2026, 08, 30, 14, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP de Itália",
                        Circuito = "Autodromo Nazionale Monza",
                        DataHora = new DateTime(2026, 09, 06, 14, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP do Azerbaijão",
                        Circuito = "Circuito de Rua de Baku",
                        DataHora = new DateTime(2026, 09, 20, 12, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP de Singapura",
                        Circuito = "Circuito de Rua de Marina Bay",
                        DataHora = new DateTime(2026, 10, 04, 13, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP dos Estados Unidos",
                        Circuito = "Circuito das Américas (COTA)",
                        DataHora = new DateTime(2026, 10, 18, 20, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP do México",
                        Circuito = "Autódromo Hermanos Rodríguez",
                        DataHora = new DateTime(2026, 10, 25, 20, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP de São Paulo",
                        Circuito = "Autódromo de Interlagos",
                        DataHora = new DateTime(2026, 11, 08, 17, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP de Las Vegas",
                        Circuito = "Circuito de Rua de Las Vegas",
                        DataHora = new DateTime(2026, 11, 21, 22, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP do Qatar",
                        Circuito = "Circuito Internacional de Lusail",
                        DataHora = new DateTime(2026, 11, 29, 17, 00, 00),
                        CampeonatoId = campeonato.Id
                    },
                    new Corrida
                    {
                        NomeGrandePremio = "GP de Abu Dhabi",
                        Circuito = "Circuito de Yas Marina",
                        DataHora = new DateTime(2026, 12, 06, 13, 00, 00),
                        CampeonatoId = campeonato.Id
                    }
                };

                context.Corridas.AddRange(corridas);
                await context.SaveChangesAsync();
            }
        }
    }
}