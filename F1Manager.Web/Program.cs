using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Hubs;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Ligação à base de dados (MySQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


// Configuração do Identity para autenticação e autorização
// todo: ajustar as opções de password e sign-in
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddRoles<IdentityRole>() // Para as roles "Admin" e "Piloto"
.AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSignalR();

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Redireciona exceções não tratadas (erros internos do servidor) para a página customizada /500 em produção
    app.UseExceptionHandler("/500");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Interceta erros de código de status (ex: 404) e redireciona para a respetiva página (/404 ou /500)
app.UseStatusCodePagesWithReExecute("/{0}");

app.UseHttpsRedirection();

app.UseRouting();


app.UseAuthentication(); // verifica QUEM é o utilizador
app.UseAuthorization(); // verifica o que o utilizador pode fazer (com base nas roles)

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllers();
app.MapHub<RaceHub>("/raceHub");


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.SeedRolesAndAdminAsync(services);
}

app.Run();
