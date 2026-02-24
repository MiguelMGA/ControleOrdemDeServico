# Instruções

- Caso o tempo não seja suficiente, priorize a **qualidade, o padrão e a estrutura do código**, definindo claramente quais funcionalidades não serão implementadas.
- Caso alguma funcionalidade não seja implementada, isso **deve ser documentado neste README**, explicando o motivo.
- O código fornecido contém **"problemas" que devem ser identificados e corrigidos**.
- Fique a vontade para criar, renomear e remover pastas,bibliotecas e até a solução não utilizadas.
- O sistema deve **compilar corretamente e executar todas as ações previstas**.
- O código final **não deve apresentar erros nem warnings** durante a compilação.
- Deve ser enviado via e-mail para consultoria com o link do projeto no Github. A consultaoria terá até terça-feira dia 13 as as 13 horas para encaminhar o e-mail.
- Utilize a extensão do SonarLint para verificar os problemas.
- Monte os testes de unidade
---

## 1. Introdução

Sistema para um prestador de serviços (ou pequena equipe) registrar clientes, abrir ordens de serviço, acompanhar status, registrar valores e anexar fotos de antes/depois do serviço.

---

## 2. Funcionalidades Detalhadas

### 2.1 Cadastro de Cliente

#### Objetivo
Permitir registrar e consultar dados do cliente para vinculação em Ordens de Serviço (OS).

#### Campos (mínimo)
- Nome (obrigatório, 2–150 caracteres)
- Id (gerado pelo sistema)
- Telefone (opcional, até 30 caracteres)
- E-mail (opcional, até 120 caracteres, formato válido)
- Documento (CPF/CNPJ) (opcional, até 30 caracteres, sem validação pesada)
- Data de criação (gerado pelo sistema)

#### Regras de Negócio
1. Nome é obrigatório e não pode conter apenas whitespace.
2. Telefone e e-mail podem ser nulos; se informados, devem ser trimados.
3. Opcionalmente, bloquear ou alertar duplicidade por:
   - Documento (CPF/CNPJ), quando informado
   - Telefone, quando informado

#### Operações
- Criar cliente
- Consultar cliente por Id
- Buscar cliente por telefone ou documento

#### Casos de Teste
- Criar cliente com nome válido retorna id
- Criar cliente sem nome retorna 400 Validation Error

---

### 2.2 Abertura de Ordem de Serviço

#### Objetivo
Criar uma OS vinculada a um cliente, com descrição e dados iniciais.

#### Campos (mínimo)
- ClienteId (obrigatório)
- Descrição do serviço (obrigatório, 1–500 caracteres)
- Número da OS (gerado automaticamente, sequencial/identity)
- Status (inicial = Aberta)
- Data de abertura (gerado pelo sistema)
- Valor do serviço (decimal(18,2)) (opcional no momento da abertura)
- Moeda (BRL)
- Data de atualização valor (opcional)

#### Regras de Negócio
1. Só é possível abrir OS para cliente existente.
2. Descrição é obrigatória.
3. Status inicial deve ser sempre Aberta.
4. Número da OS deve ser único e sequencial.
5. Regra de negócio item 2.4 

#### Operações
- Abrir OS
- Consultar OS por Id
- Listar OS por cliente, status ou período

#### Casos de Teste
- Abrir OS para cliente existente
- Abrir OS para cliente inexistente

---

### 2.3 Status da Ordem de Serviço

#### Objetivo
Permitir acompanhar o ciclo do serviço.

#### Estados
- Aberta
- Em Execução
- Finalizada

#### Regras de Transição
- Aberta -> Em Execução (permitido)
- Em Execução -> Finalizada (permitido)
- Aberta -> Finalizada (bloqueado)
- Finalizada -> qualquer outro (bloqueado)

#### Operações
- Alterar status
- Registrar datas opcionais:
  - StartedAt ao entrar em Em Execução
  - FinishedAt ao entrar em Finalizada

#### Casos de Teste
- Alterar Aberta para Em Execução retorna sucesso
- Alterar Em Execução para Finalizada retorna sucesso
- Alterar Finalizada para outro status retorna erro

---

### 2.4 Valor do Serviço

#### Objetivo
Permitir definir ou ajustar o valor do serviço.

#### Campos
- Valor (decimal(18,2))
- Moeda (BRL)
- Data de atualização (opcional)

#### Regras de Negócio
1. Valor pode ser nulo enquanto Aberta ou Em Execução.
2. Valor pode ser obrigatório para finalizar a OS.
3. Valor não pode ser negativo.
4. Após Finalizada, não permitir alteração.

#### Operações
- Definir ou alterar valor
- Validar valor ao finalizar OS

---

## 3. API Sugerida

### Clientes
- POST /v1/customers
- GET /v1/customers/{id}

### Ordens de Serviço
- POST /v1/service-orders
- GET /v1/service-orders/{id}
- PATCH /v1/service-orders/{id}/status
- PUT /v1/service-orders/{id}/price
---

## 4. Requisitos Não Funcionais (Opcional)

### Observabilidade
- Registrar logs para criação de cliente, abertura de OS e mudança de status.
---

# 5. Bônus Implementados

## 5.1 Exclusão Lógica (Soft Delete)

Foi implementada exclusão lógica para as entidades:

- **Customer**
- **ServiceOrder**

### Estratégia Utilizada

Ao invés de remover fisicamente os registros do banco de dados, foi adotada a abordagem de **Soft Delete**, onde:

- Um campo booleano `IsDeleted` foi adicionado às entidades.
- A operação HTTP `DELETE` altera `IsDeleted = true`.
- Registros excluídos não são retornados nas consultas padrão.
- A integridade histórica dos dados é preservada.

### Regras Aplicadas

#### Customer
- Não é removido fisicamente do banco.
- Não pode ser consultado após exclusão nas rotas padrão.
- **Ao excluir um Customer, todas as suas ServiceOrders também são marcadas como excluídas (exclusão lógica em cascata).**

#### ServiceOrder
- Não é removida fisicamente do banco.
- Não pode sofrer alterações após exclusão lógica.
- Não aparece nas listagens padrão de ativos.

---

## 5.2 Novas Rotas GET (Listagem)

Foram criadas duas rotas de listagem tanto para **Customer** quanto para **ServiceOrder**:

### 1️ Listar apenas registros ativos
Retorna apenas registros onde `IsDeleted = false`.

### 2️ Listar todos os registros
Retorna todos os registros, incluindo os excluídos logicamente.

Essa separação permite:

- Segurança na consulta padrão
- Auditoria administrativa quando necessário
- Preservação do histórico de dados

---

## 5.3 Documentação da API com Swagger

Foi implementada documentação automática da API utilizando **Swagger / OpenAPI**.

A documentação permite:

- Visualizar todos os endpoints disponíveis
- Testar requisições diretamente pelo navegador
- Verificar contratos de entrada e saída
- Conferir códigos de retorno HTTP

### Acesso Local

Para acessar a documentação, utilize:

http://localhost:<PORTA>/swagger

A `<PORTA>` deve ser substituída pela porta configurada na sua máquina (definida no `docker-compose` ou `launchSettings.json`).

---

## 6. Configuração de Ambiente

### 6.1 Variáveis de Ambiente (.env)

O projeto utiliza variáveis de ambiente para configuração de conexão com banco de dados, porta e outras informações sensíveis.

#### Instruções

1. Existe um arquivo de exemplo: `.env.example`
2. Para o projeto rodar corretamente, copie este arquivo e renomeie para `.env`:

cp .env.example .env

ou manualmente crie .env baseado no .env.example.

3. Ajuste os valores das variáveis conforme o seu ambiente local. O arquivo .env deve conter pelo menos:

SA_PASSWORD=SqlServer2024!Strong#
DB_HOST=sqlserver,1433
DB_NAME=OsServiceDb

Sem o arquivo .env corretamente configurado, a aplicação não irá iniciar.

SA_PASSWORD → senha do SQL Server

DB_HOST → host e porta do SQL Server (ex.: sqlserver,1433)

DB_NAME → nome do banco de dados a ser utilizado

---

## 7. Próximos Passos (Evolução do Sistema)

Como evolução natural do projeto, o próximo passo seria o desenvolvimento de um **Front-end** para consumo da API.

Isso permitiria:

- Visualização completa do fluxo de negócio
- Interface para abertura e acompanhamento de Ordens de Serviço
- Melhor experiência de usuário
- Validações adicionais no lado do cliente
- Demonstração clara da visão de produto

A ausência do front-end neste desafio ocorreu exclusivamente por limitação de tempo, priorizando:

- Arquitetura
- Qualidade do código
- Regras de negócio
- Testes de unidade
- Correções estruturais
- Documentação

---

## 8. Conformidade com as Instruções do Desafio

✔ Código compila sem erros  
✔ Sem warnings (verificado com SonarLint)  
✔ Testes de unidade implementados  
✔ Problemas estruturais identificados e corrigidos  
✔ Exclusão lógica implementada com regra de cascata  
✔ Rotas adicionais de listagem criadas  
✔ Documentação Swagger implementada  
✔ Logs aplicados nas operações críticas  
✔ Estrutura organizada por camadas (Domain, Services, Infrastructure, API)  