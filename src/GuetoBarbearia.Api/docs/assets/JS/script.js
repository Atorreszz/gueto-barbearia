const campoData = document.querySelector("#data");
const campoHorario = document.querySelector("#horario");
const resumoServico = document.querySelector("#resumo-servico");
const campoServico = document.querySelector("#servico");
const formularioAgendamento = document.querySelector("#form-agendamento");
const campoNome = document.querySelector("#nome");
const campoWhatsapp = document.querySelector("#whatsapp");
const erroNome = document.querySelector("#erro-nome");
const erroWhatsapp = document.querySelector("#erro-whatsapp");

const hoje = new Date();

const ano = hoje.getFullYear();
const mes = String(hoje.getMonth() + 1).padStart(2, "0");
const dia = String(hoje.getDate()).padStart(2, "0");

const dataMinima = `${ano}-${mes}-${dia}`;

campoData.min = dataMinima;

campoData.addEventListener("change", () => {
    const possuiDataSelecionada = campoData.value !== "";

    campoHorario.disabled = !possuiDataSelecionada;
    campoHorario.value = "";
});

const servicos = [
    {
        id: "twist",
        nome: "Twist",
        preco: 120.00,
        duracaoMinutos: 140
    },
    {
        id: "barrel",
        nome: "Barrel Twist",
        preco: 150,
        duracaoMinutos: 150
    },
    {
        id: "americano",
        nome: "Corte Disfarçado",
        preco: 25,
        duracaoMinutos: 20
    },
    {
        id: "startlocs",
        nome: "Starter Locs",
        preco: 100,
        duracaoMinutos: 110
    }
];

servicos.forEach((servico) => {
    const elemento = document.createElement("option");

    elemento.value = servico.id;
    elemento.textContent = servico.nome;

    campoServico.appendChild(elemento);
});

campoServico.addEventListener("change", () => {
    const produtoEncontrado = servicos.find((servico) => {
        return servico.id === campoServico.value;
    });

    const precoFormatado = produtoEncontrado.preco.toLocaleString("pt-BR", {
        style: "currency",
        currency: "BRL"
    });

    resumoServico.textContent = `Serviço selecionado: ${produtoEncontrado.nome}. Preço: ${precoFormatado}. Duração: ${produtoEncontrado.duracaoMinutos} minutos.`;
});

const horariosExemplos = ["09:00", "13:00", "16:00"];

horariosExemplos.forEach((horas) => {
    const hora = document.createElement("option");

    hora.value = horas;
    hora.textContent = horas;

    campoHorario.appendChild(hora);
});

formularioAgendamento.addEventListener("submit", (evento) => {
    evento.preventDefault();

    erroNome.textContent = "";
    erroWhatsapp.textContent = "";

    const nomeLimpo = campoNome.value.trim();

    if (nomeLimpo === "") {
        erroNome.textContent = "Digite seu nome.";
        return;
    }

    const whatsappLimpo = campoWhatsapp.value.replace(/\D/g, "");

    if (whatsappLimpo.length !== 11) {
        erroWhatsapp.textContent = "Digite um celular com DDD e 11 dígitos.";
        return;
    }

    const agendamento = {
        servicoId: campoServico.value,
        data: campoData.value,
        horario: campoHorario.value,
        nome: nomeLimpo,
        whatsapp: whatsappLimpo
    };

    console.log(agendamento);
});