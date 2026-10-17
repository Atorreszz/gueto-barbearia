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
        private IServicos servicosServices;

        public ServicosController(IServicos servicos)
        {
            this.servicosServices = servicos;
        }
        
        private static List<Servico> servicos = new List<Servico>()
            {
                new Servico() {Id = 1, Nome = "Twist", DuracaoMinutos = 140, Preco = 120m} 
            };
        
            [HttpGet]
            public ActionResult<List<Servico>> Get()
            {
                
            return Ok(servicosServices.ObterTodos());
        }
            [HttpGet("{id}")]
            public ActionResult<Servico> Get(int id)
            {
                var servico = servicosServices.ObterPorId(id);
                if (servico is null)
                {
                    return NotFound($"Servico Com Id {id} não Encontrado");
                }
                return Ok(servico);
            }

            [HttpPost]
            public ActionResult<Servico> Post(Servico novoServico)
            {
               servicosServices.Adicionar(novoServico);
                return Created();
            }

            [HttpPut("{id}")]
            public ActionResult<Servico> Put(int id, Servico servicoAtualizado)
            {
                var servico = servicosServices.Atualizar(id, servicoAtualizado);
                if (servico is null)
                {
                    return NotFound($"Servico com ID {id} Não Encontrado");
                }
               
                
                return Ok(servico);
                
            }

            [HttpDelete("{id}")]
            public ActionResult<Servico> Delete(int id)
            {
                var deletou = servicosServices.Remover(id);
                var servico = servicos.FirstOrDefault(x => x.Id == id);
                if (deletou == false)
                {
                    return NotFound($"Servico com ID {id} Não Encontrado");
                }
                
                return NoContent();
            }
    }
}
