using Microsoft.AspNetCore.Authentication.JwtBearer;  // Usado para a autenticação JWT
using Microsoft.IdentityModel.Tokens;                 // Usado para o manuseio de tokens e validação
using Microsoft.OpenApi.Models;                       // Usado para configurar o Swagger/OpenAPI
using System.Text;                                     // Usado para codificação de strings

var builder = WebApplication.CreateBuilder(args);      // Inicializa o construtor do aplicativo ASP.NET Core

// Adicionar serviços ao contêiner de injeção de dependência
builder.Services.AddControllers();  // Adiciona suporte para controladores MVC

// Configuração de Autenticação JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);  // Obtém a chave JWT do appsettings.json
builder.Services.AddAuthentication(options =>  // Configura a autenticação
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  // Define o esquema de autenticação padrão como JWT Bearer
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;     // Define o esquema de desafio de autenticação padrão como JWT Bearer
})
.AddJwtBearer(options =>  // Configuração específica para autenticação JWT Bearer
{
    options.RequireHttpsMetadata = false;  // Desabilita o requisito de metadados HTTPS (útil para desenvolvimento)
    options.SaveToken = true;  // Salva o token no contexto de autenticação
    options.TokenValidationParameters = new TokenValidationParameters  // Parâmetros para validação do token
    {
        ValidateIssuerSigningKey = true,  // Valida a chave de assinatura do emissor do token
        IssuerSigningKey = new SymmetricSecurityKey(key),  // Define a chave de assinatura simétrica
        ValidateIssuer = false,  // Desativa a validação do emissor (pode ser ativada para maior segurança)
        ValidateAudience = false  // Desativa a validação da audiência (pode ser ativada para maior segurança)
    };
});

// Adicionar e configurar o Swagger para documentação da API
builder.Services.AddEndpointsApiExplorer();  // Adiciona suporte para descoberta de endpoints
builder.Services.AddSwaggerGen(c =>  // Configura o Swagger
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT Auth API", Version = "v1" });  // Define informações básicas para a documentação da API

    // Configurar autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",  // Nome do cabeçalho onde o token será passado
        Type = SecuritySchemeType.ApiKey,  // Tipo de esquema de segurança como chave de API
        Scheme = "Bearer",  // Define o esquema como "Bearer"
        BearerFormat = "JWT",  // Formato esperado é JWT
        In = ParameterLocation.Header,  // Indica que o token será passado no cabeçalho da requisição
        Description = "Insira 'Bearer' [espaço] e o token JWT"  // Descrição de como fornecer o token JWT
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement  // Define o requisito de segurança para o Swagger
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,  // Tipo de referência para o esquema de segurança
                    Id = "Bearer"  // ID do esquema de segurança definido acima
                }
            },
            Array.Empty<string>()  // Escopos de segurança, vazios para autenticação básica de JWT
        }
    });
});

var app = builder.Build();  // Constrói o aplicativo com as configurações definidas

// Configurar o pipeline de requisição HTTP
if (app.Environment.IsDevelopment())  // Se o ambiente for de desenvolvimento
{
    app.UseDeveloperExceptionPage();  // Habilita a página de exceções do desenvolvedor
    app.UseSwagger();  // Habilita o middleware do Swagger para geração de documentação JSON
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWT Auth API v1"));  // Habilita a interface do usuário do Swagger
}

app.UseHttpsRedirection();  // Redireciona HTTP para HTTPS

app.UseAuthentication();  // Habilita a autenticação (deve vir antes de Authorization)
app.UseAuthorization();   // Habilita a autorização

app.MapControllers();  // Mapeia os endpoints dos controladores para as rotas de requisição

app.Run();  // Executa o aplicativo
