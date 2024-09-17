using Mailer.Middleware;
using Mailer.Service;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services au conteneur
builder.Services.AddControllersWithViews();

// Enregistrement du service d'envoi de mail
builder.Services.AddSingleton<MailerService>();

// Enregistrement du service pour la génération de tokens JWT
builder.Services.AddSingleton<JwtTokenService>();

// Stock & cache code
builder.Services.AddSingleton<TwoFactorAuthService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

// Utilisation du middleware pour les tokens JWT
app.UseMiddleware<TokenMiddleware>();

app.UseRouting();
app.UseAuthorization();


// Map des routes pour les contrôleurs, y compris l'authentification et les mails
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(name: "auth", pattern: "{controller=Auth}/{action=Login}"); // Route pour l'authentification

app.Run();
