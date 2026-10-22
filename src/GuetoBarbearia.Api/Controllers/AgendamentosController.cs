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
                return NotFound($"Agendamento com ID {id} não encontrado.");

            return Ok(agendamento);
        }

        [HttpGet("horarios-disponiveis")]
        public async Task<ActionResult<List<string>>> ObterHorariosDisponiveis(
            [FromQuery] DateOnly data,
            [FromQuery] int servicoId)
        {
            try
            {
                var horarios = await agendamentoService.ObterHorariosDisponiveis(data, servicoId);
                return Ok(horarios);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro interno ao buscar horários.", detalhe = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Agendamento>> Post(Agendamento novoAgendamento)
        {
            try
            {
                var agendamentoCriado = await agendamentoService.Adicionar(novoAgendamento);
                return CreatedAtAction(nameof(Get), new { id = agendamentoCriado.Id }, agendamentoCriado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro interno ao criar agendamento.", detalhe = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Agendamento>> Put(int id, Agendamento agendamentoAtualizado)
        {
            try
            {
                var agendamento = await agendamentoService.Atualizar(id, agendamentoAtualizado);
                if (agendamento is null)
                    return NotFound($"Agendamento com ID {id} não encontrado.");

                return Ok(agendamento);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro interno ao atualizar agendamento.", detalhe = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var cancelou = await agendamentoService.Remover(id);
                if (!cancelou)
                    return NotFound($"Agendamento com ID {id} não encontrado.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}
