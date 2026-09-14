const campoData = document.querySelector("#data");
const campoHorario = document.querySelector("#horario");
const resumoServico = document.querySelector("#resumo-servico");

const hoje = new Date();

const ano = hoje.getFullYear();
const mes = String(hoje.getMonth() + 1).padStart(2, "0");
const dia = String(hoje.getDate()).padStart(2, "0");

const dataMinima = `${ano}-${mes}-${dia}`;

campoData.min = dataMinima;

campoData.addEventListener("change", () => {
    const possuiDataSelecionada = campoData.value !== "";

    campoHorario.disabled = !possuiDataSelecionada;

    if (!possuiDataSelecionada) {
        campoHorario.value = "";
    }
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
    }
];
const campoServico = document.querySelector("#servico");

servicos.forEach(servico => {
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
    })

    resumoServico.textContent = `Serviço selecionado: ${produtoEncontrado.nome}. Preço:${precoFormatado}. Duração: ${produtoEncontrado.duracaoMinutos} minutos.`;
});

