using Microsoft.AspNetCore.Authorization;  // Necessário para a anotação [Authorize], que controla o acesso baseado em autorização
using Microsoft.AspNetCore.Mvc;            // Necessário para o uso de Controladores e a construção de APIs RESTful

// Define a rota base para o controlador como "api/[controller]", que será "api/bemvindo" devido ao nome do controlador.
[Route("api/[controller]")]
[ApiController]  // Indica que este controlador é uma API e adiciona funcionalidades automáticas como validação de modelo
public class BemVindoController : ControllerBase  // Define o controlador BemVindoController, que herda de ControllerBase
{
    // Define um endpoint HTTP GET com a rota "api/bemvindo/protected"
    [HttpGet("protected")]
    [Authorize]  // Indica que este endpoint requer autenticação; somente usuários autenticados podem acessá-lo
    public IActionResult GetProtected()  // Método que será chamado quando uma solicitação GET for feita para "api/bemvindo/protected"
    {
        return Ok("Bem-vindo! Você está autenticado.");  // Retorna uma resposta HTTP 200 (OK) com a mensagem "Bem-vindo! Você está autenticado."
    }
}
