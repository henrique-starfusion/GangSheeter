
# 🖨️ GangSheeter

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

## ✨ Funcionalidades

### 🖼️ Lista de Imagens

- Upload múltiplo de imagens
- Miniaturas interativas
- Edição de cópias, dimensões (cm) e resolução (DPI)
- Exclusão de imagens da lista
- Exibição de informações detalhadas (nome, dimensões, DPI)
- Campo quantidade de cópias editável
- Funcionalidade de arrastar e soltar (drag and drop) para adicionar imagens

### 📄 Geração de Folha de Impressão

- Largura fixa de **58 cm**
- Altura dinâmica (máximo de **1500 cm**)
- Distribuição inteligente com algoritmo de ML.NET
- Respeito ao espaçamento mínimo/máximo (1 cm a 5 cm)
- Margem de 0,5 cm para cada lado
- Ao montar a folha de impressão, repetir a imagem com base na quantidade de cópias

### 🖱️ Interação com a Folha

- Mover e rotacionar imagens manualmente
- Zoom e navegação livre
- Fundo quadriculado (checkerboard) estilo Photoshop
- Exclusão de imagens da folha

### 📤 Exportação em TIFF

- Arquivo final gerado com:
  - Fundo transparente
  - Compactação LZW
  - Resolução de 300 DPI

### ⚙️ Configurações

- Largura e altura da folha (mínima/máxima)
- DPI de exportação
- Tipo de compactação TIFF
- Persistência via SQLite

### 🧠 Aprendizado de Máquina

- O algoritmo aprende com distribuições anteriores
- Otimização contínua do aproveitamento de espaço

---

## 💾 Banco de Dados (SQLite)

Os seguintes dados são armazenados:

- Configurações do sistema
- Histórico de folhas geradas
- Imagens e suas configurações
- Dados de aprendizado para o algoritmo ML

---

## Interface

![Interface do GangSheeter](https://github.com/henrique-starfusion/GangSheeter/blob/develop/interface.png?raw=true)

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

## 🗂️ Roadmap

- [ ] Abrir multiplas imagens
- [ ] Lista de imagens abertas com quantidade de cópias e exluir imagem
- [ ] Editor visual com arrastar e rotacionar
- [ ] Salvar arquivo TIFF
- [ ] Gerar Arquivo de Impressão
- [ ] Importar várias Imagens
- [ ] Arrastar e Solvar Imagens
- [ ] Reorganizar Imagens
- [ ] Tela de Configuração
- [ ] Aplicar Camada Spot
- [ ] Criar arquivo RIP
- [ ] Multi idiomas
- [ ] Abrir todas as imagens de uma pasta
- [ ] Editar imagem da lista
- [ ] Alterar tamanho da imagem
- [ ] Alterar resolução da imagem
- [ ] Remover fundo da imagem

---

## 📃 Licença

Distribuído sob a licença MIT. Veja `LICENSE` para mais detalhes.

---

## 🙌 Contribuições

Contribuições são bem-vindas! Abra uma issue ou envie um pull request. 😉

---

Nome: Henrique Rodrigues
Empresa: StarFusion
Site: www.starfusion.com.br

---
