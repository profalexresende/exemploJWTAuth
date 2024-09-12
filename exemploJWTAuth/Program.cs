using Microsoft.AspNetCore.Authentication.JwtBearer;  // Usado para a autentica��o JWT
using Microsoft.IdentityModel.Tokens;                 // Usado para o manuseio de tokens e valida��o
using Microsoft.OpenApi.Models;                       // Usado para configurar o Swagger/OpenAPI
using System.Text;                                     // Usado para codifica��o de strings

var builder = WebApplication.CreateBuilder(args);      // Inicializa o construtor do aplicativo ASP.NET Core

// Adicionar servi�os ao cont�iner de inje��o de depend�ncia
builder.Services.AddControllers();  // Adiciona suporte para controladores MVC

// Configura��o de Autentica��o JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);  // Obt�m a chave JWT do appsettings.json
builder.Services.AddAuthentication(options =>  // Configura a autentica��o
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  // Define o esquema de autentica��o padr�o como JWT Bearer
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;     // Define o esquema de desafio de autentica��o padr�o como JWT Bearer
})
.AddJwtBearer(options =>  // Configura��o espec�fica para autentica��o JWT Bearer
{
    options.RequireHttpsMetadata = false;  // Desabilita o requisito de metadados HTTPS (�til para desenvolvimento)
    options.SaveToken = true;  // Salva o token no contexto de autentica��o
    options.TokenValidationParameters = new TokenValidationParameters  // Par�metros para valida��o do token
    {
        ValidateIssuerSigningKey = true,  // Valida a chave de assinatura do emissor do token
        IssuerSigningKey = new SymmetricSecurityKey(key),  // Define a chave de assinatura sim�trica
        ValidateIssuer = false,  // Desativa a valida��o do emissor (pode ser ativada para maior seguran�a)
        ValidateAudience = false  // Desativa a valida��o da audi�ncia (pode ser ativada para maior seguran�a)
    };
});

// Adicionar e configurar o Swagger para documenta��o da API
builder.Services.AddEndpointsApiExplorer();  // Adiciona suporte para descoberta de endpoints
builder.Services.AddSwaggerGen(c =>  // Configura o Swagger
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT Auth API", Version = "v1" });  // Define informa��es b�sicas para a documenta��o da API

    // Configurar autentica��o JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",  // Nome do cabe�alho onde o token ser� passado
        Type = SecuritySchemeType.ApiKey,  // Tipo de esquema de seguran�a como chave de API
        Scheme = "Bearer",  // Define o esquema como "Bearer"
        BearerFormat = "JWT",  // Formato esperado � JWT
        In = ParameterLocation.Header,  // Indica que o token ser� passado no cabe�alho da requisi��o
        Description = "Insira 'Bearer' [espa�o] e o token JWT"  // Descri��o de como fornecer o token JWT
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement  // Define o requisito de seguran�a para o Swagger
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,  // Tipo de refer�ncia para o esquema de seguran�a
                    Id = "Bearer"  // ID do esquema de seguran�a definido acima
                }
            },
            Array.Empty<string>()  // Escopos de seguran�a, vazios para autentica��o b�sica de JWT
        }
    });
});

var app = builder.Build();  // Constr�i o aplicativo com as configura��es definidas

// Configurar o pipeline de requisi��o HTTP
if (app.Environment.IsDevelopment())  // Se o ambiente for de desenvolvimento
{
    app.UseDeveloperExceptionPage();  // Habilita a p�gina de exce��es do desenvolvedor
    app.UseSwagger();  // Habilita o middleware do Swagger para gera��o de documenta��o JSON
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWT Auth API v1"));  // Habilita a interface do usu�rio do Swagger
}

app.UseHttpsRedirection();  // Redireciona HTTP para HTTPS

app.UseAuthentication();  // Habilita a autentica��o (deve vir antes de Authorization)
app.UseAuthorization();   // Habilita a autoriza��o

app.MapControllers();  // Mapeia os endpoints dos controladores para as rotas de requisi��o

app.Run();  // Executa o aplicativo
