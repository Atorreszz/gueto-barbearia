const campoData = document.querySelector("#data");
const campoHorario = document.querySelector("#horario");
const resumoServico = document.querySelector("#resumo-servico");
const campoServico = document.querySelector("#servico");
const formularioAgendamento = document.querySelector("#form-agendamento");
const campoNome = document.querySelector("#nome");
const campoWhatsapp = document.querySelector("#whatsapp");
const erroNome = document.querySelector("#erro-nome");
const erroWhatsapp = document.querySelector("#erro-whatsapp");
const resumoAgendamento = document.querySelector("#resumo-agendamento");


const hoje = new Date();

const ano = hoje.getFullYear();
const mes = String(hoje.getMonth() + 1).padStart(2, "0");
const dia = String(hoje.getDate()).padStart(2, "0");

const dataMinima = `${ano}-${mes}-${dia}`;

campoData.min = dataMinima;


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


const horariosExemplos = [
    "09:00",
    "13:00",
    "16:00"
];


function buscarServico(idServico) {
    return servicos.find((servico) => {
        return servico.id === idServico;
    });
}


function formatarData(data) {
    const [
        anoAgendamento,
        mesAgendamento,
        diaAgendamento
    ] = data.split("-");

    return `${diaAgendamento}/${mesAgendamento}/${anoAgendamento}`;
}


function validarWhatsapp(whatsapp) {
    return whatsapp.length === 11;
}


function formatarWhatsapp(whatsapp) {
    whatsapp = whatsapp.replace(/\D/g, "");

    return whatsapp.replace(
        /(\d{2})(\d{5})(\d{4})/,
        "($1) $2-$3"
    );
}


function exibirResumo(agendamento) {
    const dataFormatada = formatarData(agendamento.data);

    resumoAgendamento.innerHTML = `
        <h3>Resumo do agendamento</h3>

        <p>
            <strong>Cliente:</strong>
            ${agendamento.nome}
        </p>

        <p>
            <strong>Serviço:</strong>
            ${agendamento.servico}
        </p>

        <p>
            <strong>Data:</strong>
            ${dataFormatada}
        </p>

        <p>
            <strong>Horário:</strong>
            ${agendamento.horario}
        </p>
    `;
}


servicos.forEach((servico) => {
    const elemento = document.createElement("option");

    elemento.value = servico.id;
    elemento.textContent = servico.nome;

    campoServico.appendChild(elemento);
});


horariosExemplos.forEach((horas) => {
    const hora = document.createElement("option");

    hora.value = horas;
    hora.textContent = horas;

    campoHorario.appendChild(hora);
});


campoData.addEventListener("change", () => {
    const possuiDataSelecionada = campoData.value !== "";

    campoHorario.disabled = !possuiDataSelecionada;
    campoHorario.value = "";
});


campoServico.addEventListener("change", () => {
    const produtoEncontrado = buscarServico(campoServico.value);

    const precoFormatado = produtoEncontrado.preco.toLocaleString(
        "pt-BR",
        {
            style: "currency",
            currency: "BRL"
        }
    );

    resumoServico.textContent = `
        Serviço selecionado: ${produtoEncontrado.nome}.
        Preço: ${precoFormatado}.
        Duração: ${produtoEncontrado.duracaoMinutos} minutos.
    `;
});


campoWhatsapp.addEventListener("input", () => {
    campoWhatsapp.value = formatarWhatsapp(campoWhatsapp.value);
});


formularioAgendamento.addEventListener("submit", (evento) => {
    evento.preventDefault();

    erroNome.textContent = "";
    erroWhatsapp.textContent = "";
    resumoAgendamento.innerHTML = "";

    const nomeLimpo = campoNome.value.trim();

    if (nomeLimpo === "") {
        erroNome.textContent = "Digite seu nome.";
        return;
    }


    const whatsappLimpo = campoWhatsapp.value.replace(/\D/g, "");

    if (!validarWhatsapp(whatsappLimpo)) {
        erroWhatsapp.textContent =
            "Digite um celular com DDD e 11 dígitos.";

        return;
    }


    const servicoSelecionado = buscarServico(campoServico.value);

    const agendamento = {
        servicoId: campoServico.value,
        servico: servicoSelecionado.nome,
        data: campoData.value,
        horario: campoHorario.value,
        nome: nomeLimpo,
        whatsapp: whatsappLimpo
    };


    exibirResumo(agendamento);

    console.log(agendamento);
});