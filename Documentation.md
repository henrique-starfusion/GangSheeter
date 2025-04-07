# GangSheeter Documentation

## 🖨️ GangSheeter

**GangSheeter** é um sistema desktop em WPF (.NET 9) voltado para gráficas e profissionais de impressão, que permite gerar folhas DTF otimizadas com base em imagens carregadas e um algoritmo inteligente de distribuição utilizando **ML.NET**.

---

## 🧰 Tecnologias Utilizadas

- [.NET 9](https://dotnet.microsoft.com/)
- [WPF (MVVM)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/)
- [SQLite](https://www.sqlite.org/index.html)
- [ML.NET](https://dotnet.microsoft.com/en-us/apps/machinelearning-ai/ml-dotnet)
- [MediatR](https://github.com/jbogard/MediatR)

---

## 📦 Estrutura do Projeto

```
GangSheeter.sln
│
├── GangSheeter.App           -> Interface WPF e camada de apresentação
├── GangSheeter.Domain        -> Entidades e regras de domínio
├── GangSheeter.Application   -> Comandos, Queries, Handlers (MediatR)
├── GangSheeter.Infrastructure-> Banco de dados, ML.NET, persistência
```

---

## ✨ Funcionalidades

### 🖼️ Gerenciamento de Imagens

- Carregar várias imagens
- Miniaturas interativas
- Edição de cópias, dimensões (cm) e resolução (DPI)
- Remoção individual

### 📄 Geração de Folha de Impressão

- Largura fixa de **58 cm**
- Altura dinâmica (máximo de **1500 cm**)
- Distribuição inteligente com algoritmo de ML.NET
- Respeito ao espaçamento mínimo/máximo (1 cm a 5 cm)

### 🧠 Aprendizado de Máquina

- O algoritmo aprende com distribuições anteriores
- Otimização contínua do aproveitamento de espaço

### 🖱️ Interação com a Folha

- Mover e rotacionar imagens manualmente
- Zoom e navegação livre
- Fundo quadriculado (checkerboard) estilo Photoshop

### 📤 Exportação TIFF

- Arquivo final gerado com:
  - Fundo transparente
  - Compactação LZW
  - Resolução de 300 DPI

### ⚙️ Configurações

- Largura e altura da folha (mínima/máxima)
- DPI de exportação
- Tipo de compactação TIFF
- Persistência via SQLite

---

## 🚀 Como Executar

1. Clone o repositório:

   ```bash
   git clone https://github.com/henrique-starfusion/GangSheeter.git
   cd GangSheeter
   ```

2. Restaure os pacotes:

   ```bash
   dotnet restore
   ```

3. Execute o projeto:

   ```bash
   dotnet run --project GangSheeter.App
   ```

---

## 📋 Instruções de Uso

1. **Carregar Imagens**: Clique no botão "Carregar Imagens" para selecionar e carregar várias imagens. As imagens aparecerão em uma lista interativa com miniaturas.

2. **Gerar Folha de Impressão**: Após carregar as imagens, clique no botão "Gerar Folha" para criar uma prévia da folha de impressão. O sistema distribuirá as imagens conforme os critérios definidos.

3. **Reorganizar Imagens**: Clique no botão "Reorganizar" para utilizar o modelo de aprendizado de máquina e reorganizar as imagens de forma diferente das tentativas anteriores.

4. **Salvar Imagem**: Após gerar a folha, clique no botão "Salvar Imagem" para exportar a folha de impressão como um arquivo TIFF.

---

## 💾 Banco de Dados (SQLite)

Os seguintes dados são armazenados:

- Configurações do sistema
- Histórico de folhas geradas
- Imagens e suas configurações
- Dados de aprendizado para o algoritmo ML

---

## 📃 Licença

Distribuído sob a licença MIT. Veja `LICENSE` para mais detalhes.

---

## 🙌 Contribuições

Contribuições são bem-vindas! Abra uma issue ou envie um pull request. 😉
