using GuetoBarbearia.Api.models;
using GuetoBarbearia.Api.Services;
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

        [HttpGet]
        public async Task<ActionResult<List<Agendamento>>> Get()
        {
            var agendamentos = await agendamentoService.ObterTodos();
            return Ok(agendamentos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Agendamento>> Get(int id)
        {
            var agendamento = await agendamentoService.ObterPorId(id);
            if (agendamento is null)
            {
                return NotFound($"Agendamento Com Id {id} não Encontrado");
            }
            return Ok(agendamento);
        }

        [HttpPost]
        public async Task<ActionResult<Agendamento>> Post(Agendamento novoAgendamento)
        {
            var agendamentoCriado = await agendamentoService.Adicionar(novoAgendamento);
            return CreatedAtAction(nameof(Get), new { id = agendamentoCriado.Id }, agendamentoCriado);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Agendamento>> Put(int id, Agendamento agendamentoAtualizado)
        {
            var agendamento = await agendamentoService.Atualizar(id, agendamentoAtualizado);
            if (agendamento is null)
            {
                return NotFound($"Agendamento com ID {id} Não Encontrado");
            }
            return Ok(agendamento);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deletou = await agendamentoService.Remover(id);
            if (!deletou)
            {
                return NotFound($"Agendamento com ID {id} não encontrado.");
            }
            return NoContent();
        }
    }
}