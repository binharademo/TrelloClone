# Ementa do Curso: Desenvolvimento Web com Blazor WebAssembly

## Descrição do Curso

Este curso abrangente ensina o desenvolvimento de aplicações web modernas usando Blazor WebAssembly, com foco em aplicações de gerenciamento de tarefas estilo Kanban. Os alunos aprenderão a criar interfaces interativas, implementar comunicação em tempo real, e desenvolver aplicações web completas utilizando C# tanto no frontend quanto no backend.

## Pré-requisitos

- Conhecimento básico de C#
- Familiaridade com HTML, CSS e JavaScript
- Compreensão básica de desenvolvimento web

## Módulo 1: Introdução ao Blazor WebAssembly

1. **Fundamentos do Blazor**
   - Arquitetura do Blazor WebAssembly
   - Comparação com outras frameworks frontend
   - Ciclo de vida de uma aplicação Blazor

2. **Configuração do Ambiente de Desenvolvimento**
   - Instalação do .NET SDK
   - Criação de projetos Blazor WebAssembly
   - Estrutura de arquivos e pastas
   - Análise do arquivo Program.cs e configuração inicial

3. **Componentes Blazor**
   - Anatomia de um componente Razor
   - Ciclo de vida dos componentes
   - Renderização e re-renderização
   - Importações com _Imports.razor

## Módulo 2: Componentes e UI

1. **Criação de Componentes Reutilizáveis**
   - Componentes aninhados
   - Passagem de parâmetros
   - Eventos e callbacks
   - Componentes genéricos

2. **Layouts e Roteamento**
   - Configuração de rotas com @page
   - Layouts compartilhados
   - NavMenu e navegação
   - Parâmetros de rota

3. **Formulários e Validação**
   - EditForm e InputBase
   - Validação com DataAnnotations
   - Validação personalizada
   - Feedback visual para o usuário

4. **Estilização e Bootstrap**
   - Integração com Bootstrap
   - CSS isolado para componentes
   - Responsividade
   - Temas e personalização

## Módulo 3: Gerenciamento de Estado e Comunicação

1. **Gerenciamento de Estado**
   - Estado do componente vs. estado da aplicação
   - Serviços injetáveis para estado compartilhado
   - Padrões de gerenciamento de estado
   - StateHasChanged e renderização manual

2. **Comunicação HTTP**
   - HttpClient em Blazor
   - Consumo de APIs RESTful
   - Serialização e desserialização JSON
   - Tratamento de erros e timeouts

3. **Autenticação e Autorização**
   - Implementação de login e registro
   - JWT e armazenamento seguro de tokens
   - Proteção de rotas
   - Contexto de autenticação

4. **LocalStorage e Persistência de Dados**
   - Acesso ao localStorage via JSInterop
   - Persistência de preferências do usuário
   - Armazenamento de tokens de autenticação
   - Estratégias de cache

## Módulo 4: Comunicação em Tempo Real

1. **Introdução ao SignalR**
   - Fundamentos de comunicação em tempo real
   - Configuração do cliente SignalR em Blazor
   - Conexão e reconexão automática
   - Grupos e comunicação direcionada

2. **Implementação de Funcionalidades Colaborativas**
   - Notificações em tempo real
   - Atualizações de UI automáticas
   - Sincronização entre múltiplos usuários
   - Tratamento de conflitos

3. **Eventos e Delegados em Blazor**
   - Sistema de eventos em C#
   - Criação e consumo de eventos personalizados
   - Padrão Observer em aplicações Blazor
   - Comunicação entre componentes não relacionados

## Módulo 5: Interatividade Avançada

1. **Drag and Drop**
   - Implementação de funcionalidade drag-and-drop nativa
   - Interação com eventos JavaScript
   - Persistência de alterações via API
   - Feedback visual durante operações de arrastar

2. **Interoperabilidade JavaScript (JSInterop)**
   - Chamada de funções JavaScript a partir do C#
   - Chamada de métodos .NET a partir do JavaScript
   - Referências a elementos do DOM
   - Bibliotecas JavaScript de terceiros

3. **Componentes Dinâmicos**
   - Renderização dinâmica de componentes
   - Componentes genéricos e tipados
   - Fábricas de componentes
   - Lazy loading

## Módulo 6: Diagnóstico e Depuração

1. **Logging e Diagnóstico**
   - Implementação de serviços de logging
   - Níveis de log e filtragem
   - Integração com console do navegador
   - Ferramentas de diagnóstico remoto

2. **Depuração de Aplicações Blazor**
   - Uso do depurador do navegador
   - Depuração de código C# no browser
   - Pontos de interrupção e inspeção de variáveis
   - Ferramentas de performance

3. **Tratamento de Erros**
   - Estratégias de tratamento de exceções
   - Componentes de erro personalizados
   - Fallbacks e degradação graciosa
   - Monitoramento de erros em produção

## Módulo 7: Otimização e Implantação

1. **Otimização de Performance**
   - Estratégias de renderização eficiente
   - Virtualização para grandes conjuntos de dados
   - Lazy loading de assemblies
   - Minimização do tamanho do download

2. **Progressive Web Apps (PWA)**
   - Transformando aplicações Blazor em PWAs
   - Service workers e cache
   - Instalação e funcionamento offline
   - Notificações push

3. **Implantação e Hospedagem**
   - Opções de hospedagem para Blazor WebAssembly
   - Configuração para ambientes de produção
   - Integração contínua e entrega contínua (CI/CD)
   - Monitoramento em produção

## Projeto Final

Os alunos desenvolverão um clone do Trello utilizando Blazor WebAssembly, implementando todas as funcionalidades aprendidas durante o curso:

- Interface de usuário responsiva com Bootstrap
- Autenticação e autorização com JWT
- Comunicação em tempo real com SignalR
- Funcionalidade drag-and-drop para cartões Kanban
- Persistência de dados via API RESTful
- Diagnóstico e logging
- Otimização para produção

## Metodologia

- Aulas teóricas e práticas
- Exercícios guiados
- Projetos incrementais
- Sessões de revisão de código
- Desenvolvimento do projeto final em etapas

## Avaliação

- Participação nas aulas (10%)
- Exercícios práticos (30%)
- Projetos incrementais (30%)
- Projeto final (30%)

## Recursos Adicionais

- Documentação oficial do Blazor
- Repositório de código do projeto Trello Clone
- Fórum de discussão para dúvidas e colaboração
- Materiais complementares e tutoriais
