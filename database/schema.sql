CREATE TABLE servicos (
    id TEXT PRIMARY KEY,
    nome TEXT NOT NULL,
    preco NUMERIC(10, 2) NOT NULL CHECK (preco > 0),
d   duracao_minutos INTEGER NOT NULL CHECK (duracao_minutos > 0)
);

CREATE TABLE agendamentos (
    id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nome_cliente TEXT NOT NULL,
    telefone_cliente TEXT NOT NULL,
    servico_id TEXT NOT NULL REFERENCES servicos(id),
    data DATE NOT NULL,
    horario TIME NOT NULL,
    UNIQUE (data, horario)
);

