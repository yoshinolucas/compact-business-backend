using System.Text;
using backend_dotnet.Config;
using backend_dotnet.Interfaces;
using backend_dotnet.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var myPolicy = "_myPolicy";
builder.Services.AddCors(options => {
    options.AddPolicy(myPolicy,
    policy => {
        policy.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var key = Encoding.ASCII.GetBytes(ConfigGlobal.Secret);

builder.Services.AddAuthentication(x => {
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters{
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IAuthRepository, AuthRepository>();
    builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>();
    builder.Services.AddScoped<IDocumentosRepository, DocumentosRepository>();
    builder.Services.AddScoped<IAlteracoesRepository, AlteracoesRepository>();
    builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(myPolicy);

app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();



app.Run();
