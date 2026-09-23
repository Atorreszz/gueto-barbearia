using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuetoBarbearia.Api.models
{
    public class Agendamento
    {
        public int Id {get;set;}
        public string NomeCliente {get;set;} = "";
        public string TelefoneCliente {get;set;} = "";
        public string ServicoId {get;set;} = "";
        public DateOnly Data {get;set;}
        public TimeOnly Horario {get;set;}
    }
}