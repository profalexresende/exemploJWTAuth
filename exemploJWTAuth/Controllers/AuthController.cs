using Microsoft.AspNetCore.Mvc;  // Necessário para o uso de controladores e construção de APIs RESTful
using Microsoft.IdentityModel.Tokens;  // Necessário para trabalhar com tokens de segurança, incluindo JWT
using System;  // Inclui funcionalidades básicas como DateTime
using System.IdentityModel.Tokens.Jwt;  // Inclui classes para manipulação de tokens JWT
using System.Security.Claims;  // Inclui classes para manipulação de declarações de segurança (claims)
using System.Text;  // Necessário para manipulação de strings, como a codificação de chaves

// Define a rota base para o controlador como "api/[controller]", que será "api/auth" devido ao nome do controlador
[Route("api/[controller]")]
[ApiController]  // Indica que este controlador é uma API e adiciona funcionalidades automáticas, como validação de modelo
public class AuthController : ControllerBase  // Define o controlador AuthController, que herda de ControllerBase
{
    // Define um endpoint HTTP POST com a rota "api/auth/login"
    [HttpPost("login")]
    public IActionResult Login()  // Método que será chamado quando uma solicitação POST for feita para "api/auth/login"
    {
        // Aqui você pode adicionar lógica de validação de usuário (autenticação real)
        var tokenHandler = new JwtSecurityTokenHandler();  // Instancia um manipulador de token JWT
        var key = Encoding.ASCII.GetBytes("lBMj-Hgsm5_kwgw0DhrAqCseujVUA78_NkPKKQneB7mgyVfA7Lwp7ocOvqldouyp");  // Define a chave de segurança usada para assinar o token JWT

        // Descritor do token que contém as informações sobre o token, como claims, data de expiração, e credenciais de assinatura
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "user") }),  // Define a identidade do token com uma claim de nome de usuário "user"
            Expires = DateTime.UtcNow.AddHours(1),  // Define a data de expiração do token para 1 hora a partir do momento atual
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)  // Define o algoritmo de assinatura e a chave de segurança
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);  // Cria o token JWT com base no descritor definido
        var tokenString = tokenHandler.WriteToken(token);  // Gera a string do token JWT

        return Ok(new { Token = tokenString });  // Retorna o token JWT como resposta JSON, com um status HTTP 200 (OK)
    }
}
