using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GuetoBarbearia.Api;
using GuetoBarbearia.Api.Data;
using GuetoBarbearia.Api.Services;

namespace GuetoBarbearia.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicosController : ControllerBase
    {
        private readonly IServicos servicosServices;

        public ServicosController(IServicos servicos)
        {
            this.servicosServices = servicos;
        }

        [HttpGet]
        public async Task<ActionResult<List<Servico>>> Get()
        {
            var servicos = await servicosServices.ObterTodos();
            return Ok(servicos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Servico>> Get(int id)
        {
            var servico = await servicosServices.ObterPorId(id);
            if (servico is null)
            {
                return NotFound($"Servico Com Id {id} não Encontrado");
            }
            return Ok(servico);
        }

        [HttpPost]
        public async Task<ActionResult<Servico>> Post(Servico novoServico)
        {
            try
            {
                var servicoCriado = await servicosServices.Adicionar(novoServico);
                return CreatedAtAction(nameof(Get), new { id = servicoCriado.Id }, servicoCriado);
            }
            catch (Exception exeption)
            {
                return BadRequest(exeption.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Servico>> Put(int id, Servico servicoAtualizado)
        {
            try
            {


                var servico = await servicosServices.Atualizar(id, servicoAtualizado);
                if (servico is null)
                {
                    return NotFound($"Servico com ID {id} Não Encontrado");
                }

                return Ok(servico);
            }
            catch (Exception exeption)
            {
                return BadRequest(exeption.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deletou =  await servicosServices.Remover(id);
            if (!deletou)
            {
                return NotFound($"Servico com ID {id} Não Encontrado");
            }
            
            return NoContent();
        }
    }
}
