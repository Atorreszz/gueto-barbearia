using GuetoBarbearia.Api.models;
using GuetoBarbearia.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace GuetoBarbearia.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgendamentosController : ControllerBase
    {
        private readonly IAgendamento agendamentoService;

        public AgendamentosController(IAgendamento agendamento)
        {
            this.agendamentoService = agendamento;
        }
        private static List<Agendamento> agendamentos = new List<Agendamento>()
        {
            new Agendamento() { Data = new DateOnly(2026,10,18), Horario = new TimeOnly(09, 00), Id = 1, NomeCliente = "Arthur",
                ServicoId = "1", TelefoneCliente = "24998372214" }
        };
        [HttpGet]
        public ActionResult<List<Agendamento>> Get()
        {
                
            return Ok(agendamentoService.ObterTodos());
        }
        [HttpGet("{id}")]
        public ActionResult<Agendamento> Get(int id)
        {
            var agendamento = agendamentoService.ObterPorId(id);
            if (agendamento is null)
            {
                return NotFound($"Agendamento Com Id {id} não Encontrado");
            }
            return Ok(agendamento);
        }

        [HttpPost]
        public ActionResult<Agendamento> Post(Agendamento novoAgendamento)
        {
            agendamentoService.Adicionar(novoAgendamento);
            return Created();
        }

        [HttpPut("{id}")]
        public ActionResult<Agendamento> Put(int id, Agendamento agendamentoAtualizado)
        {
            var agendamento = agendamentoService.Atualizar(id, agendamentoAtualizado);
            if (agendamento is null)
            {
                return NotFound($"Agendamento com ID {id} Não Encontrado");
            }
               
                
            return Ok(agendamento);
                
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var deletou = agendamentoService.Remover(id);
            if (!deletou)
            {
                return NotFound($"Agendamento com ID {id} não encontrado.");
            }
                
            return NoContent();
        }
    }
}
