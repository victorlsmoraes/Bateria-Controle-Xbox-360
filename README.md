# Bateria-Controle-Xbox-360

--------------------------------


🔋 Sobre o Projeto
Este é um utilitário leve e prático desenvolvido em C# com .NET 8 para usuários de controles de Xbox 360 no Windows. Diferente dos controles de Xbox One/Series, o Windows nem sempre mostra de forma clara o nível de carga das baterias dos controles de 360, e este software resolve exatamente esse problema.

✨ Funcionalidades
Monitoramento em Tempo Real: Verifique o status da bateria sem precisar abrir menus complexos.

Alertas Visuais: Ícones intuitivos que mudam de cor/forma conforme o nível de carga (Cheio, Médio, Baixo e Crítico).

Interface Minimalista: Criado para ser leve e não interferir no desempenho dos seus jogos.


🚀 Como Usar
Vá até a aba Releases aqui no GitHub.

Baixe o arquivo .zip da versão mais recente.

Extraia o conteúdo para uma pasta de sua preferência.

Execute o arquivo Bateria Controle Xbox 360.exe.

Nota: Caso o Windows exiba um aviso de "Fornecedor Desconhecido", clique em "Mais informações" e "Executar assim mesmo".


🛠️ Tecnologias Utilizadas
Linguagem: C#

Framework: .NET 8.0 (Windows Forms)

Arquitetura: x64 (Autossuficiente)


💡 Dica de Ouro: Adicione imagens!
No GitHub, uma imagem vale mais que mil palavras de código. Como você já tem os ícones (battery_full, battery_low, etc.), você pode criar uma pequena tabela no seu README para mostrar o que cada ícone significa:

Status:		ícone:		

Cheio		      🟢

Médio		      🟡

Baixo 	          🟠 

Vazio		      🔴
    
<img width="747" height="418" alt="image" src="https://github.com/user-attachments/assets/e305c213-6dc8-43aa-8f9c-ca2f4bd31af3" />

--------------------------------


## 🛠️ Funções de Inicialização e Interface

### `Form1()`
É o **construtor** do aplicativo. Ele prepara o terreno: esconde a janela inicialmente para evitar "pulos" visuais, arredonda as barras de bateria, configura o menu da bandeja (systray) e inicia a tela de carregamento.

### `CriarLoadingScreen()`
Cria dinamicamente um painel que cobre toda a interface ao abrir o app. Ela configura a logo, o título e as mensagens de "Aguarde", garantindo que o usuário tenha um feedback visual enquanto o hardware é acessado.

### `IniciarAppAsync()`
Controla o tempo de exibição do loading. Ela espera alguns segundos (delay), marca o carregamento como concluído e faz uma animação suave (fade-out e subida) para revelar o dashboard principal.

### `AdicionarMarcaDagua()`
Insere um selo de identificação no canto inferior da tela. Serve para exibir a versão atual do software (**v0.1.0-BETA**) e os seus créditos de desenvolvedor.

---

## 🎮 Funções de Comunicação com o Controle (XInput)

### `XInputGetState` & `XInputGetBatteryInformation` (Nativos)
Não são funções escritas por você, mas sim "pontes" para as bibliotecas oficiais do Windows. Elas permitem que o C# pergunte ao sistema: *"O controle está ligado?"* e *"Qual o nível exato da bateria agora?"*.

### `timer1_Tick()`
É o **coração** do programa. Ele roda várias vezes por segundo para:
1. Verificar se o controle ainda está conectado.
2. Ler o nível de bateria atualizado.
3. Atualizar o cronômetro de tempo de uso.
4. Chamar as funções que atualizam as cores e o ícone na bandeja.

### `AtualizarStatusBateria()`
Recebe o nível da bateria (0 a 3) e decide como a interface deve se comportar. Ela define o texto (Ex: "Médio:") e pinta as barras de progresso com as cores correspondentes (Vermelho, Laranja, Amarelo ou Verde).

---

## 🎨 Funções de Design e Efeitos

### `ArredondarPainel()`
Aplica um "corte" geométrico nos cantos dos painéis. É o que transforma os retângulos rígidos em barras modernas com cantos arredondados.

### `AplicarOpacidade()`
Manipula a imagem do controle via código. Ela é usada para criar aquele efeito de "controle apagado" quando desconectado e "controle brilhante" quando conectado.

### `TimerTransicao_Tick()`
Faz a animação suave da opacidade. Em vez de a imagem do controle mudar bruscamente, esta função faz ela clarear ou escurecer gradualmente.

---

## 📋 Funções de Sistema e Utilidade

### `ConfigurarBandeja()` & `AtualizarIconeBandeja()`
Gerenciam a presença do app na área de ícones do Windows (perto do relógio). Permitem que você veja a bateria sem abrir a janela e oferecem o menu de "Sair" ou "Abrir".

### `PosicionarNoCanto()` & `RestaurarJanela()`
Controlam a física da janela. Garantem que o app sempre abra no canto inferior direito (acima do relógio) e que ele retorne para lá sempre que for minimizado ou restaurado.

### `VerificarAtalhoTeclado()`
Monitora as entradas do controle em tempo real. Se você apertar a combinação de botões definida (**LT + RT + Back + Start**), ela decide se deve esconder o app ou trazê-lo para a frente.

### `ImageFromBytes()`
Uma função auxiliar de memória. Ela converte os arquivos de imagem que estão guardados dentro do seu projeto em objetos que o Windows consegue desenhar na tela de forma segura.

---
