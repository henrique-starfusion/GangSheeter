# Documentação do GangSheeter

## Índice

1. [Visão Geral](#visão-geral)
2. [Tecnologias Utilizadas](#tecnologias-utilizadas)
3. [Funcionalidades](#funcionalidades)
   - [Lista de Imagens](#lista-de-imagens)
   - [Geração de Folha de Impressão](#geração-de-folha-de-impressão)
   - [Interação com a Folha](#interação-com-a-folha)
   - [Exportação em TIFF](#exportação-em-tiff)
   - [Configurações](#configurações)
   - [Aprendizado de Máquina](#aprendizado-de-máquina)
4. [Banco de Dados](#banco-de-dados)
5. [Interface do Usuário](#interface-do-usuário)
6. [Como Executar](#como-executar)
7. [Contribuições](#contribuições)
8. [Licença](#licença)

## Visão Geral

**GangSheeter** é um sistema desktop desenvolvido em WPF (.NET 9) voltado para gráficas e profissionais de impressão. O software permite gerar folhas DTF otimizadas com base em imagens carregadas e um algoritmo inteligente de distribuição utilizando **ML.NET**.

## Tecnologias Utilizadas

- [.NET 9](https://dotnet.microsoft.com/)
- [WPF (MVVM)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/)
- [SQLite](https://www.sqlite.org/index.html)
- [ML.NET](https://dotnet.microsoft.com/en-us/apps/machinelearning-ai/ml-dotnet)
- [MediatR](https://github.com/jbogard/MediatR)

## Funcionalidades

### Lista de Imagens
- **Upload Múltiplo de Imagens:** Permite que o usuário carregue várias imagens de uma vez.
- **Miniaturas Interativas:** Exibe miniaturas das imagens carregadas para fácil visualização.
- **Edição de Cópias, Dimensões e Resolução:** O usuário pode editar a quantidade de cópias, dimensões (cm) e resolução (DPI) de cada imagem.
- **Exclusão de Imagens:** Permite que o usuário remova imagens da lista.
- **Informações Detalhadas:** Exibe informações como nome, dimensões e DPI das imagens.
- **Campo de Quantidade de Cópias Editável:** O usuário pode definir a quantidade de cópias desejadas.
- **Arrastar e Soltar:** Implementa a funcionalidade de arrastar e soltar para adicionar imagens.

### Geração de Folha de Impressão
- **Largura Fixa:** A largura da folha é fixa em 58 cm.
- **Altura Dinâmica:** A altura é dinâmica, com um máximo de 1500 cm.
- **Distribuição Inteligente:** Utiliza um algoritmo de ML.NET para otimizar a distribuição das imagens.
- **Espaçamento e Margens:** Respeita o espaçamento mínimo/máximo (1 cm a 5 cm) e uma margem de 0,5 cm para cada lado.
- **Repetição de Imagens:** As imagens são repetidas com base na quantidade de cópias ao montar a folha.

### Interação com a Folha
- **Mover e Rotacionar Imagens:** Permite que o usuário mova e rotacione as imagens manualmente.
- **Zoom e Navegação Livre:** O usuário pode aplicar zoom e navegar livremente pela folha.
- **Fundo Quadriculado:** Adiciona um fundo quadriculado (checkerboard) estilo Photoshop.
- **Exclusão de Imagens da Folha:** Permite que o usuário remova imagens da folha de impressão.

### Exportação em TIFF
- **Arquivo Final:** Gera um arquivo TIFF com fundo transparente, compactação LZW e resolução de 300 DPI.

### Configurações
- **Configuração de Largura e Altura:** Permite que o usuário defina a largura e altura da folha (mínima/máxima).
- **Configuração de DPI e Compactação:** Permite que o usuário configure o DPI de exportação e o tipo de compactação TIFF.
- **Persistência via SQLite:** Armazena as configurações no banco de dados SQLite.

### Aprendizado de Máquina
- **Otimização Contínua:** O algoritmo aprende com distribuições anteriores para otimizar o aproveitamento de espaço.

## Banco de Dados

O GangSheeter utiliza SQLite para armazenar:
- Configurações do sistema.
- Histórico de folhas geradas.
- Imagens e suas configurações.
- Dados de aprendizado para o algoritmo ML.

## Interface do Usuário

A interface do GangSheeter é projetada para ser amigável e intuitiva, permitindo que os usuários naveguem facilmente pelas funcionalidades do software.

![Interface do GangSheeter](https://github.com/henrique-starfusion/GangSheeter/blob/develop/interface.png?raw=true)

## Como Executar

1. Clone o repositório:
   ```bash
   git clone https://github.com/henrique-starfusion/GangSheeter.git
   cd