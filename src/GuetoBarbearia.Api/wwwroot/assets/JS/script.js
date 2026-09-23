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
const erroData = document.querySelector("#erro-data");
const erroHorario = document.querySelector("#erro-horario");

function obterDataAtual() {
    const hoje = new Date();

    const ano = hoje.getFullYear();
    const mes = String(hoje.getMonth() + 1).padStart(2, "0");
    const dia = String(hoje.getDate()).padStart(2, "0");

    return `${ano}-${mes}-${dia}`;
}

campoData.min = obterDataAtual();

const servicos = [
    {
        id: "twist",
        nome: "Twist",
        preco: 120,
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
            <span id="resumo-cliente"></span>
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

    const nomeResumo = document.querySelector("#resumo-cliente");

    nomeResumo.textContent = agendamento.nome;
}

function ehDomingo(data) {
    const [ano, mes, dia] = data.split("-");

    const dataEscolhida = new Date(
        Number(ano),
        Number(mes) - 1,
        Number(dia)
    );

    return dataEscolhida.getDay() === 0;
}

function ehHoje(data) {
    return data === obterDataAtual();
}

function horarioJaPassou(horario) {
    const [hora, minuto] = horario.split(":");

    const agora = new Date();

    const minutosAgora =
        agora.getHours() * 60 + agora.getMinutes();

    const minutosHorario =
        Number(hora) * 60 + Number(minuto);

    return minutosHorario <= minutosAgora;
}

function obterHorariosDisponiveis(data) {
    if (!ehHoje(data)) {
        return horariosExemplos;
    }

    return horariosExemplos.filter((horario) => {
        return !horarioJaPassou(horario);
    });
}

function atualizarHorarios(data) {
    campoHorario.innerHTML = `
        <option value="" selected disabled>
            Selecione um horário
        </option>
    `;

    const horariosDisponiveis = obterHorariosDisponiveis(data);

    if (horariosDisponiveis.length === 0) {
        campoHorario.innerHTML = `
            <option value="" selected disabled>
                Nenhum horário disponível
            </option>
        `;

        campoHorario.disabled = true;
        return;
    }

    campoHorario.disabled = false;

    horariosDisponiveis.forEach((horario) => {
        const opcao = document.createElement("option");

        opcao.value = horario;
        opcao.textContent = horario;

        campoHorario.appendChild(opcao);
    });
}

servicos.forEach((servico) => {
    const elemento = document.createElement("option");

    elemento.value = servico.id;
    elemento.textContent = servico.nome;

    campoServico.appendChild(elemento);
});

campoData.addEventListener("change", () => {
    const dataAtual = obterDataAtual();

    campoData.min = dataAtual;
    erroData.textContent = "";
    erroHorario.textContent = "";

    campoHorario.innerHTML = `
        <option value="" selected disabled>
            Selecione um horário
        </option>
    `;

    campoHorario.disabled = true;

    if (campoData.value === "") {
        return;
    }

    if (campoData.value < dataAtual) {
        erroData.textContent = "Escolha uma data a partir de hoje.";
        return;
    }

    if (ehDomingo(campoData.value)) {
        erroData.textContent =
            "Não realizamos agendamentos aos domingos.";

        return;
    }

    atualizarHorarios(campoData.value);
});

campoServico.addEventListener("change", () => {
    const produtoEncontrado = buscarServico(campoServico.value);

    if (!produtoEncontrado) {
        resumoServico.textContent = "";
        return;
    }

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
    campoWhatsapp.value = formatarWhatsapp(
        campoWhatsapp.value
    );

    const whatsappLimpo = campoWhatsapp.value.replace(/\D/g, "");

    if (validarWhatsapp(whatsappLimpo)) {
        erroWhatsapp.textContent = "";
    }
});

campoNome.addEventListener("input", () => {
    if (campoNome.value.trim() !== "") {
        erroNome.textContent = "";
    }
});

formularioAgendamento.addEventListener("input", () => {
    resumoAgendamento.textContent = "";
});

formularioAgendamento.addEventListener("reset", () => {
    campoData.min = obterDataAtual();

    erroNome.textContent = "";
    erroData.textContent = "";
    erroWhatsapp.textContent = "";
    erroHorario.textContent = "";

    resumoServico.textContent = "";
    resumoAgendamento.textContent = "";

    campoHorario.innerHTML = `
        <option value="" selected disabled>
            Selecione um horário
        </option>
    `;

    campoHorario.disabled = true;
});

formularioAgendamento.addEventListener("submit", (evento) => {
    evento.preventDefault();

    const dataAtual = obterDataAtual();

    campoData.min = dataAtual;

    erroNome.textContent = "";
    erroWhatsapp.textContent = "";
    erroData.textContent = "";
    erroHorario.textContent = "";
    resumoAgendamento.textContent = "";

    const nomeLimpo = campoNome.value.trim();

    if (nomeLimpo === "") {
        erroNome.textContent = "Digite seu nome.";
        campoNome.focus();
        return;
    }

    const whatsappLimpo =
        campoWhatsapp.value.replace(/\D/g, "");

    if (!validarWhatsapp(whatsappLimpo)) {
        erroWhatsapp.textContent =
            "Digite um celular com DDD e 11 dígitos.";

        campoWhatsapp.focus();
        return;
    }

    if (campoData.value === "") {
        erroData.textContent = "Escolha uma data.";
        campoData.focus();
        return;
    }

    if (campoData.value < dataAtual) {
        erroData.textContent = "Escolha uma data a partir de hoje.";
        campoHorario.value = "";
        campoHorario.disabled = true;
        campoData.focus();
        return;
    }

    if (ehDomingo(campoData.value)) {
        erroData.textContent =
            "Não realizamos agendamentos aos domingos.";

        campoHorario.value = "";
        campoHorario.disabled = true;
        campoData.focus();
        return;
    }

    if (campoHorario.value === "") {
        if (campoHorario.disabled) {
            erroHorario.textContent =
                "Não há horários disponíveis para essa data. Escolha outra data.";

            campoData.focus();
        } else {
            erroHorario.textContent = "Selecione um horário.";
            campoHorario.focus();
        }

        return;
    }

    if (ehHoje(campoData.value) && horarioJaPassou(campoHorario.value)) {
        erroHorario.textContent =
            "Esse horário já passou. Escolha outro horário ou outra data.";

        atualizarHorarios(campoData.value);

        if (campoHorario.disabled) {
            campoData.focus();
        } else {
            campoHorario.focus();
        }

        return;
    }

    const servicoSelecionado =
        buscarServico(campoServico.value);

    if (!servicoSelecionado) {
        resumoServico.textContent = "Selecione um serviço.";
        campoServico.focus();
        return;
    }

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