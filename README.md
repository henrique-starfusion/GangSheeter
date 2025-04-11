# GangSheeter - Gerador de Folhas de Impressão

Aplicação WPF para geração de folhas de impressão para DTF.

## Funcionalidades Principais

1. Selecionar Imagens para Impressão
    A aplicação deve ter um botão para selecionar as imagens que serão impressas.
    Ao selecionar as imagens elas devem aparecer em um grid onde tera uma miniatura da imagem, um campo com a quantidade (campo editável), o nome do arquivo, a pasta do arquivo, a resolução da imagem (DPI), a largura (em centimetros), a altura (em centimetros), o tamanho da imagem, e um botão para deletar a imagem.
    Na primeira coluna da lista de imagens deve ter um checkbox para poder selecionar as imagens.

2. Gerar Folha de Impressão    
    A aplicação deve ter um botão para gerar a folha de impressão.
    O Sistema irá pegar as imagens selecionadas para poder gerar as folhas, também irá levar em consideração a quantidade de cópias de cada imagem, as imagens da lista que já foram para o arquivo de impressão, devem estar marcadas e o checkbox deve estar desabilitado.
    Ao clicar no botão, o sistema deve colocar os arquivos em uma área onde o usuário podera mover e rotacionar as imagens para encaixar na folha, a folha deve ter 58cm de largura e a altura variável, não podendo ultrapassar os 15 metros de altura.

3. Reorganizar Imagens da Folha
    A aplicação deve distribuir as imagens na folha de forma a encaixar todas as imagens na folha, de forma a ocupar a menor altura possível, as imagens podem ser ajustadas tanto na horizontal quanto na vertical, porem elas não podem ficar uma em cima da outra, uma imagem deve ter no mínimo 1cm de espaço entre as outras e no máximo 10cm de espaço uma das outras.
    O sistema deve usar uma IA para reorganizar as imagens e aprender qual a melhor forma de aproveitar ao máximo a folha de impressão.

4. Salvar Folha de Impressão
    A aplicação deve ter um botão para salvar a folha de impressão, a folha deve ser salva no formato tiff e com fundo transparente, utilizar a compressão LZW e a imagem deve ter 300 DPI.
    Ao salvar uma folha a IA deve aprender com o resultado e melhorar a reorganização das imagens na folha de impressão.

5. Folha de Impressão / Visualização Prévia
    Deve ser pacecido com a area de edição do photoshop, toca quadriculada e deve ter 58cm de largura e a altura variável, não podendo ultrapassar os 15 metros de altura.
    Deve permitir dar zoom na folha de impressão.


## Requisitos do Sistema
- .NET 9.0
- Windows 10/11

## Desenvolvimento
Projeto desenvolvido em WPF com:
- Fluent.Ribbon para interface moderna
- Padrão MVVM para arquitetura limpa
- Data binding para atualizações em tempo real
- SQLite para banco de dados
- ML.NET para IA
- MediatR