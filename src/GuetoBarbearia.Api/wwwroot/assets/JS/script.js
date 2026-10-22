const campoData         = document.querySelector("#data");
const campoHorario      = document.querySelector("#horario");
const campoServico      = document.querySelector("#servico");
const campoNome         = document.querySelector("#nome");
const campoWhatsapp     = document.querySelector("#whatsapp");
const resumoServico     = document.querySelector("#resumo-servico");
const resumoAgendamento = document.querySelector("#resumo-agendamento");
const erroNome          = document.querySelector("#erro-nome");
const erroWhatsapp      = document.querySelector("#erro-whatsapp");
const erroData          = document.querySelector("#erro-data");
const erroHorario       = document.querySelector("#erro-horario");
const erroGlobal        = document.querySelector("#erro-global");
const formularioAgendamento = document.querySelector("#form-agendamento");
const botaoTema         = document.getElementById("botao-tema");
const Tema              = document.documentElement;

let servicosCarregados = [];

botaoTema.addEventListener("click", () => {
    const temaAtual = Tema.getAttribute("data-tema");
    if (temaAtual === "escuro") {
        Tema.setAttribute("data-tema", "claro");
        botaoTema.textContent = "Ativar Modo Escuro";
    } else {
        Tema.setAttribute("data-tema", "escuro");
        botaoTema.textContent = "Ativar Modo Claro";
    }
});

function obterDataAtual() {
    const hoje = new Date();
    const ano  = hoje.getFullYear();
    const mes  = String(hoje.getMonth() + 1).padStart(2, "0");
    const dia  = String(hoje.getDate()).padStart(2, "0");
    return `${ano}-${mes}-${dia}`;
}

function formatarData(data) {
    const [ano, mes, dia] = data.split("-");
    return `${dia}/${mes}/${ano}`;
}

function formatarPreco(valor) {
    return Number(valor).toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

function limparErros() {
    erroNome.textContent     = "";
    erroWhatsapp.textContent = "";
    erroData.textContent     = "";
    erroHorario.textContent  = "";
    if (erroGlobal) erroGlobal.textContent = "";
}

function mostrarErroGlobal(mensagem) {
    if (erroGlobal) {
        erroGlobal.textContent = mensagem;
        erroGlobal.scrollIntoView({ behavior: "smooth", block: "center" });
    }
}

async function carregarServicos() {
    try {
        const resposta = await fetch("/api/servicos");

        if (!resposta.ok) {
            throw new Error(`Erro ao carregar serviços: ${resposta.status}`);
        }

        const servicos = await resposta.json();
        servicosCarregados = servicos;

        campoServico.innerHTML = `<option value="" selected disabled>Selecione um serviço</option>`;

        servicos.forEach((servico) => {
            const opcao = document.createElement("option");
            opcao.value = servico.id;
            opcao.textContent = servico.nome;
            campoServico.appendChild(opcao);
        });

    } catch (erro) {
        console.error("Falha ao carregar serviços da API:", erro);
        campoServico.innerHTML = `<option value="" disabled>Erro ao carregar serviços. Tente recarregar a página.</option>`;
    }
}

async function carregarHorariosDisponiveis(data, servicoId) {
    campoHorario.innerHTML = `<option value="" selected disabled>Carregando horários...</option>`;
    campoHorario.disabled = true;
    erroHorario.textContent = "";

    try {
        const url = `/api/agendamentos/horarios-disponiveis?data=${data}&servicoId=${servicoId}`;
        const resposta = await fetch(url);

        if (!resposta.ok) {
            const corpo = await resposta.json().catch(() => ({}));
            throw new Error(corpo.erro || `Erro ${resposta.status}`);
        }

        const horarios = await resposta.json();

        campoHorario.innerHTML = `<option value="" selected disabled>Selecione um horário</option>`;

        if (horarios.length === 0) {
            campoHorario.innerHTML = `<option value="" selected disabled>Nenhum horário disponível para esta data</option>`;
            erroHorario.textContent = "Não há horários disponíveis para esta data. Tente outra data.";
            return;
        }

        horarios.forEach((horario) => {
            const opcao = document.createElement("option");
            opcao.value = horario;
            opcao.textContent = horario;
            campoHorario.appendChild(opcao);
        });

        campoHorario.disabled = false;

    } catch (erro) {
        console.error("Falha ao buscar horários:", erro);
        campoHorario.innerHTML = `<option value="" selected disabled>Erro ao buscar horários</option>`;
        erroHorario.textContent = erro.message || "Erro ao buscar horários disponíveis.";
    }
}

async function enviarAgendamento(dados) {
    try {
        const resposta = await fetch("/api/agendamentos", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                nomeCliente:     dados.nome,
                telefoneCliente: dados.whatsapp,
                servicoId:       Number(dados.servicoId),
                data:            dados.data,
                horario:         dados.horario + ":00"
            })
        });

        const corpo = await resposta.json();

        if (!resposta.ok) {
            mostrarErroGlobal(corpo.erro || "Erro ao confirmar agendamento.");
            return false;
        }

        exibirResumo(dados, corpo);
        return true;

    } catch (erro) {
        console.error("Erro de rede ao enviar agendamento:", erro);
        mostrarErroGlobal("Erro de conexão. Verifique sua internet e tente novamente.");
        return false;
    }
}

function exibirResumo(dados, agendamentoCriado) {
    const servico = servicosCarregados.find(s => s.id === Number(dados.servicoId));
    const nomeServico = servico ? servico.nome : "Serviço";
    const dataFormatada = formatarData(dados.data);

    const numeroPH = "5524999405127";
    const mensagem = encodeURIComponent(
        `Olá PH! Sou ${dados.nome}, acabei de agendar um *${nomeServico}* para *${dataFormatada}* às *${dados.horario}*. Pode confirmar? 😊`
    );
    const linkWhatsApp = `https://wa.me/${numeroPH}?text=${mensagem}`;

    resumoAgendamento.innerHTML = `
        <h3>✅ Agendamento confirmado!</h3>
        <p><strong>Cliente:</strong> <span>${agendamentoCriado.nomeCliente || dados.nome}</span></p>
        <p><strong>Serviço:</strong> ${nomeServico}</p>
        <p><strong>Data:</strong> ${dataFormatada}</p>
        <p><strong>Horário:</strong> ${dados.horario}</p>
        <p class="resumo-info">Guarde estas informações! Para cancelar ou tirar dúvidas, entre em contato com o PH.</p>
        <a href="${linkWhatsApp}" target="_blank" rel="noopener noreferrer" class="botao botao-whatsapp">
            💬 Confirmar via WhatsApp com o PH
        </a>
    `;
}

function validarFormulario(dados) {
    let valido = true;
    limparErros();

    if (!dados.nome || dados.nome.trim().length < 3) {
        erroNome.textContent = "Digite seu nome completo (mínimo 3 caracteres).";
        campoNome.focus();
        valido = false;
    }

    if (!dados.whatsapp || dados.whatsapp.length !== 11) {
        erroWhatsapp.textContent = "Digite um celular com DDD e 11 dígitos (ex: 24999405127).";
        if (valido) campoWhatsapp.focus();
        valido = false;
    }

    if (!dados.servicoId) {
        resumoServico.textContent = "Selecione um serviço.";
        if (valido) campoServico.focus();
        valido = false;
    }

    if (!dados.data) {
        erroData.textContent = "Escolha uma data.";
        if (valido) campoData.focus();
        valido = false;
    }

    if (!dados.horario) {
        erroHorario.textContent = "Selecione um horário.";
        if (valido) campoHorario.focus();
        valido = false;
    }

    return valido;
}

campoData.min = obterDataAtual();

campoServico.addEventListener("change", () => {
    const servicoId = campoServico.value;
    const servico = servicosCarregados.find(s => s.id === Number(servicoId));

    if (!servico) {
        resumoServico.textContent = "";
        return;
    }

    resumoServico.textContent = `${servico.nome} — ${formatarPreco(servico.preco)} — ${servico.duracaoMinutos} min`;

    if (campoData.value) {
        carregarHorariosDisponiveis(campoData.value, servicoId);
    }
});

campoData.addEventListener("change", () => {
    erroData.textContent    = "";
    erroHorario.textContent = "";
    resumoAgendamento.textContent = "";

    campoHorario.innerHTML = `<option value="" selected disabled>Selecione um horário</option>`;
    campoHorario.disabled = true;

    const dataEscolhida = campoData.value;
    if (!dataEscolhida) return;

    if (dataEscolhida < obterDataAtual()) {
        erroData.textContent = "Escolha uma data a partir de hoje.";
        return;
    }

    if (!campoServico.value) {
        erroHorario.textContent = "Selecione um serviço primeiro para ver os horários disponíveis.";
        return;
    }

    carregarHorariosDisponiveis(dataEscolhida, campoServico.value);
});

campoWhatsapp.addEventListener("input", () => {
    let apenasDigitos = campoWhatsapp.value.replace(/\D/g, "").slice(0, 11);

    if (apenasDigitos.length <= 2) {
        campoWhatsapp.value = apenasDigitos;
    } else if (apenasDigitos.length <= 7) {
        campoWhatsapp.value = `(${apenasDigitos.slice(0, 2)}) ${apenasDigitos.slice(2)}`;
    } else {
        campoWhatsapp.value = `(${apenasDigitos.slice(0, 2)}) ${apenasDigitos.slice(2, 7)}-${apenasDigitos.slice(7)}`;
    }

    erroWhatsapp.textContent = "";
});

campoNome.addEventListener("input", () => {
    if (campoNome.value.trim().length >= 3) {
        erroNome.textContent = "";
    }
});

formularioAgendamento.addEventListener("input", () => {
    resumoAgendamento.textContent = "";
    if (erroGlobal) erroGlobal.textContent = "";
});

formularioAgendamento.addEventListener("reset", () => {
    campoData.min = obterDataAtual();
    limparErros();
    resumoServico.textContent     = "";
    resumoAgendamento.textContent = "";
    campoHorario.innerHTML = `<option value="" selected disabled>Selecione um horário</option>`;
    campoHorario.disabled = true;
});

formularioAgendamento.addEventListener("submit", async (evento) => {
    evento.preventDefault();

    const whatsappLimpo = campoWhatsapp.value.replace(/\D/g, "");

    const dados = {
        nome:      campoNome.value.trim(),
        whatsapp:  whatsappLimpo,
        servicoId: campoServico.value,
        data:      campoData.value,
        horario:   campoHorario.value
    };

    if (!validarFormulario(dados)) return;

    const botaoSubmit = formularioAgendamento.querySelector("[type=submit]");
    botaoSubmit.disabled = true;
    botaoSubmit.textContent = "Aguarde...";

    const sucesso = await enviarAgendamento(dados);

    botaoSubmit.disabled = false;
    botaoSubmit.textContent = "Confirmar agendamento";

    if (sucesso) {
        formularioAgendamento.reset();
    }
});

document.querySelectorAll('a[href^="#"]').forEach((link) => {
    link.addEventListener("click", (evento) => {
        const idAlvo = link.getAttribute("href");
        if (idAlvo && idAlvo !== "#") {
            const elementoAlvo = document.querySelector(idAlvo);
            if (elementoAlvo) {
                evento.preventDefault();
                elementoAlvo.scrollIntoView({ behavior: "smooth" });
                history.pushState(null, "", idAlvo);
            }
        }
    });
});

carregarServicos();
