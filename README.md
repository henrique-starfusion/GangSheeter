Crie um sistema desktop em WPF para geração de folhas de impressão DTF com os seguintes requisitos:

Nome do sistema GangSheeter

Nome da classe de imagem da Lista de imagens ImageItem
Nome da classe de imagem da folha ImageSheet
Nome da classe de geração da folha Sheet

Todas as classes devem estar em ingles e o sistema deve estar em pt-BR

O sistema deve tem uma interface clean e de fácil utilização.

Linguagem: C#
Framework: .NET 9

Tecnologias adicionais:
SQLite (persistência de dados)
MediatR (mediador de mensagens/eventos)
ML.NET (machine learning)

1. Upload e Gerenciamento de Imagens
O sistema deve permitir que o usuário carregue múltiplas imagens em uma lista interativa. Cada item da lista deve conter:
Miniatura da imagem.
Campo editável para definir a quantidade de cópias.
Dimensões da imagem (largura e altura em centímetros).
Resolução da imagem (em DPI).
Botão para excluir a imagem da lista.

2. Geração da Folha de Impressão
A folha de impressão deve ter:
Largura fixa de 58 cm.
Altura variável, com limite máximo de 15 metros (1500 cm).

A distribuição das imagens deve:
Aproveitar ao máximo a largura disponível da folha.
Respeitar espaçamento mínimo de 1 cm e máximo de 5 cm entre as imagens.
Ser feita por um algoritmo de machine learning com ML.NET, que deve aprender com os layouts anteriores para melhorar continuamente a eficiência da distribuição.

3. Interação com a Folha
A visualização da folha de impressão deve permitir:
Movimentar e rotacionar imagens livremente.
Zoom in/out para melhor visualização.
Exibir um fundo quadriculado (checkerboard) semelhante ao do Photoshop para facilitar a visualização de áreas transparentes.

4. Funcionalidades Principais
Botão "Gerar Folha"
Gera uma prévia da folha de impressão, distribuindo as imagens conforme os critérios definidos.
Ao gerar a folha, o sistema deve automaticamente acionar a funcionalidade de reorganização inteligente.

Botão "Reorganizar"
Utiliza um modelo de machine learning (ML.NET) para reorganizar as imagens de forma diferente das tentativas anteriores.
O sistema deve registrar as distribuições anteriores para evitar repetições e aprimorar o aprendizado.

Botão "Imprimir"
Gera um arquivo TIFF único, contendo:
As imagens organizadas conforme a disposição final.
Fundo transparente.
Compactação LZW.
Resolução de 300 DPI.
Esse arquivo será utilizado para a impressão final da folha.
Os dados de distribuição gerados devem ser armazenados para alimentar o algoritmo de aprendizado e otimizar as futuras gerações.

5. Tela de Configurações
Deve conter os seguintes parâmetros configuráveis:
Largura da folha (em cm).
Altura máxima e mínima da folha (em cm).
Resolução de exportação (em DPI).
Tipo de compactação do arquivo TIFF.
Essas configurações devem ser armazenadas no banco SQLite e utilizadas como padrão nas próximas sessões, podendo ser alteradas a qualquer momento.

6. Banco de Dados
O sistema deve utilizar SQLite para persistência dos seguintes dados:
Configurações do sistema.
Histórico de folhas geradas.
Dados de aprendizado para o algoritmo de distribuição:
Layouts anteriores.
Tempo de geração.
Eficiência do uso de espaço.
Quantidade de cópias por imagem.
Parâmetros de organização aplicados.

O objeto da lista de imagens será o nome de ImageItem e as imagens da folha terá o nome de ImageSheet o nome da folha de impressão será sheet