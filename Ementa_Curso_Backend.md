# Ementa do Curso: Desenvolvimento de Backend com ASP.NET Core para Aplicações Kanban

## Módulo 1: Arquitetura de APIs RESTful
- **Introdução ao padrão REST**
  - Princípios e constraints REST
  - Modelagem de recursos e endpoints
  - Códigos de status HTTP e seu uso adequado
- **Implementação com ASP.NET Core**
  - Estrutura de Controllers e Actions
  - Atributos de roteamento e verbos HTTP
  - Injeção de dependência em Controllers

## Módulo 2: Autenticação e Autorização
- **JSON Web Tokens (JWT)**
  - Estrutura e funcionamento dos tokens JWT
  - Implementação de autenticação baseada em tokens
  - Configuração do middleware JwtBearer
- **Segurança em APIs**
  - Hashing de senhas com SHA256
  - Proteção contra ataques comuns
  - Validação de modelos e dados de entrada

## Módulo 3: Persistência de Dados com Entity Framework Core
- **Modelagem de dados para aplicações Kanban**
  - Entidades e relacionamentos (User, Board, List, Card)
  - Configuração de relacionamentos um-para-muitos
  - Chaves estrangeiras e integridade referencial
- **SQLite como banco de dados embarcado**
  - Configuração e inicialização do banco
  - Migrações e seed de dados
  - Consultas e operações CRUD

## Módulo 4: Comunicação em Tempo Real com SignalR
- **Fundamentos do SignalR**
  - Hubs e grupos
  - Conexões persistentes e fallback
  - Autenticação em conexões SignalR
- **Implementação de funcionalidades colaborativas**
  - Notificações de movimentação de cards
  - Atualizações em tempo real do quadro Kanban
  - Gerenciamento de conexões de usuários

## Módulo 5: Logging e Diagnóstico
- **Estratégias de logging**
  - Configuração do sistema de logging
  - Níveis de log e seu uso apropriado
  - Armazenamento de logs em arquivos
- **Monitoramento de aplicações**
  - Logging de comandos SQL
  - Registro de requisições e respostas HTTP
  - Diagnóstico de problemas de autenticação

## Módulo 6: Padrões de Design e Arquitetura
- **Injeção de Dependência**
  - Serviços Scoped, Transient e Singleton
  - Implementação de interfaces para testabilidade
  - Configuração do container DI
- **Padrão Repository e Service**
  - Separação de responsabilidades
  - Implementação de serviços de domínio
  - Abstração do acesso a dados

## Projeto Final
- Desenvolvimento de uma API completa para um sistema de gerenciamento de projetos
- Implementação de autenticação, persistência e comunicação em tempo real
- Documentação com Swagger e testes automatizados
