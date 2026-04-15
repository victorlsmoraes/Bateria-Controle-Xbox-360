using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Bateria_Controle_Xbox_360
{
    public partial class Form1 : Form
    {
        // --- CHAMADAS NATIVAS DO WINDOWS (APIs do Sistema) ---

        // Permite definir uma "região" customizada para a janela (usada para arredondar)
        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        // Cria a forma geométrica de um retângulo com cantos arredondados
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        // Obtém o estado atual dos botões e analógicos do controle
        [DllImport("xinput1_4.dll")]
        static extern int XInputGetState(int dwUserIndex, out XINPUT_STATE pState);

        // Obtém especificamente as informações de bateria (tipo e nível)
        [DllImport("xinput1_4.dll")]
        static extern int XInputGetBatteryInformation(int dwUserIndex, byte devType, out XINPUT_BATTERY_INFORMATION pBatteryInformation);

        // Estruturas de dados para organizar as informações recebidas do controle
        struct XINPUT_STATE { public uint dwPacketNumber; public XINPUT_GAMEPAD Gamepad; }
        struct XINPUT_GAMEPAD { public ushort wButtons; public byte bLeftTrigger; public byte bRightTrigger; public short sThumbLX; public short sThumbLY; public short sThumbRX; public short sThumbRY; }
        struct XINPUT_BATTERY_INFORMATION { public byte BatteryType; public byte BatteryLevel; }

        // --- VARIÁVEIS GLOBAIS ---
        Panel?[]? barrasControles;      // Array para agrupar as 4 barrinhas de nível
        Panel? pnlLoading;             // Painel da tela de carregamento inicial
        DateTime inicioUso = DateTime.Now; // Marca a hora que o app abriu para o cronômetro
        private bool botaoPressionado = false; // Trava para o atalho de teclado não repetir infinito
        private float opacidade = 0.2f;        // Opacidade inicial do controle (apagado)
        private float loadingOpacity = 1f;     // Opacidade do painel de loading
        private System.Windows.Forms.Timer timerTransicao; // Timer para o efeito de fade do controle
        private bool estaConectado = false;    // Armazena o último estado de conexão
        private bool carregamentoConcluido = false; // Indica se o loading já acabou

        public Form1()
        {
            // Inicia o formulário invisível e no modo manual para evitar "pulos" na tela
            this.Opacity = 0;
            this.StartPosition = FormStartPosition.Manual;

            InitializeComponent();

            // Agrupa os painéis criados no Designer para facilitar a pintura em loop
            barrasControles = new Panel[] { bar1, bar2, bar3, bar4 };

            // Configura o timer que faz o efeito de clarear/escurecer o desenho do controle
            timerTransicao = new System.Windows.Forms.Timer { Interval = 30 };
            timerTransicao.Tick += TimerTransicao_Tick;

            // Define o estilo da janela: borda fixa e remove botão de maximizar
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Aplica o arredondamento nas 4 barrinhas de bateria
            ArredondarPainel(bar1, 10);
            ArredondarPainel(bar2, 10);
            ArredondarPainel(bar3, 10);
            ArredondarPainel(bar4, 10);

            // 1 - Inicia as configurações de bandeja e posicionamento
            // 3 - Cria a interface visual do loading
            // 4 - Adiciona a marca d'água de versão no canto inferior
            // 5 - Configuração da bolinha de informação 
            ConfigurarBandeja();
            PosicionarNoCanto();
            CriarLoadingScreen();
            AdicionarMarcaDagua();
            ConfigurarIconeInfo();

            // Inicia a tarefa assíncrona que controla o tempo do loading
            IniciarAppAsync();

        }

        // Constrói o visual da tela de carregamento inicial
        private void ConfigurarIconeInfo()
        {
            lblInfoIcon.Text = "ⓘ"; // Caractere Unicode elegante para informação
            lblInfoIcon.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblInfoIcon.ForeColor = Color.FromArgb(120, 120, 120); // Cinza discreto
            lblInfoIcon.BackColor = Color.Transparent;
            lblInfoIcon.Cursor = Cursors.Hand;
            lblInfoIcon.AutoSize = true;
            lblInfoIcon.Click += (s, e) => {
                string sobre = "XBOX 360 BATTERY DASHBOARD\n" +
                               "Versão: 0.1.0-BETA\n\n" +
                               "Desenvolvido por: Victor Leonardi\n" +
                               "Finalidade: Monitoramento de bateria para controles XInput.\n\n" +
                               "Aviso Legal: Projeto independente sem fins lucrativos. " +
                               "Xbox 360 e Microsoft são marcas registradas. " +
                               "Este software não possui vínculo oficial com a marca.";

                MessageBox.Show(sobre, "Sobre o Aplicativo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // Posiciona no canto superior direito (25 pixels da borda direita, 10 do topo)
            lblInfoIcon.Location = new Point(this.ClientSize.Width - 35, 10);

            // Configura o Balão de Texto (ToolTip)
            toolTipInfo.ToolTipTitle = "Informação";
            toolTipInfo.ToolTipIcon = ToolTipIcon.Info;
            toolTipInfo.IsBalloon = true;
            toolTipInfo.SetToolTip(lblInfoIcon, "Software independente desenvolvido para a comunidade. Não licenciado ou afiliado à Microsoft.");

            // Efeitos Visuais ao passar o mouse
            lblInfoIcon.MouseEnter += (s, e) => { lblInfoIcon.ForeColor = Color.White; };
            lblInfoIcon.MouseLeave += (s, e) => { lblInfoIcon.ForeColor = Color.FromArgb(120, 120, 120); };

            this.Controls.Add(lblInfoIcon);
            lblInfoIcon.BringToFront(); // Garante que fique acima de outros elementos do Dashboard
        }

        private void CriarLoadingScreen()
        {
            pnlLoading = new Panel();
            pnlLoading.Size = this.ClientSize;
            pnlLoading.Location = new Point(0, 0);
            pnlLoading.BackColor = Color.FromArgb(25, 25, 25);
            pnlLoading.Name = "pnlLoading";

            // Configura a imagem da logo central
            PictureBox picLogo = new PictureBox();
            picLogo.Image = ImageFromBytes(Properties.Resources.Logo_Icon);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.Size = new Size(80, 80);
            picLogo.Location = new Point((pnlLoading.Width - picLogo.Width) / 2, (pnlLoading.Height / 2) - 100);

            // Configura o título principal do loading
            Label lblLogo = new Label();
            lblLogo.Text = "XBOX 360\nBATTERY DASHBOARD";
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            lblLogo.ForeColor = Color.White;
            lblLogo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblLogo.AutoSize = false;
            lblLogo.Size = new Size(300, 60);
            lblLogo.Location = new Point((pnlLoading.Width - lblLogo.Width) / 2, picLogo.Bottom + 10);

            // Configura o texto de status do loading
            Label lblAguarde = new Label();
            lblAguarde.Text = "Sincronizando com hardware...";
            lblAguarde.ForeColor = Color.Gray;
            lblAguarde.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblAguarde.AutoSize = true;
            lblAguarde.Location = new Point((pnlLoading.Width - 160) / 2, lblLogo.Bottom + 10);

            pnlLoading.Controls.Add(picLogo);
            pnlLoading.Controls.Add(lblLogo);
            pnlLoading.Controls.Add(lblAguarde);

            this.Controls.Add(pnlLoading);
            pnlLoading.BringToFront(); // Garante que o loading fique por cima de tudo
        }

        // Método auxiliar para converter bytes dos recursos em imagens utilizáveis
        private Image? ImageFromBytes(byte[]? bytes)
        {
            if (bytes == null) return null;
            using (var ms = new MemoryStream(bytes))
            using (var tmp = Image.FromStream(ms))
            {
                return new Bitmap(tmp); // Retorna uma cópia da imagem para não travar o arquivo
            }
        }

        // Lógica assíncrona para finalizar o loading com animação
        private async void IniciarAppAsync()
        {
            this.Opacity = 1; // Torna o formulário visível (mostrando o loading)
            lblInfoIcon.Visible = false; // Esconde a bolinha durante o loading

            await Task.Delay(2500); // Aguarda 2,5 segundos para charme visual
            carregamentoConcluido = true;

            // Animação de saída: sobe o painel e diminui a cor (fade out fake)
            for (int i = 0; i < 10; i++)
            {
                if (pnlLoading != null)
                {
                    pnlLoading.Top -= 5;
                    loadingOpacity -= 0.1f;
                    int alpha = (int)(loadingOpacity * 255);
                    alpha = Math.Clamp(alpha, 0, 255);
                    pnlLoading.BackColor = Color.FromArgb(alpha, 25, 25, 25);
                    pnlLoading.Refresh();
                }
                await Task.Delay(20);
            }

            if (pnlLoading != null)
            {
                pnlLoading.Visible = false;
                this.Controls.Remove(pnlLoading);
            }

            lblInfoIcon.Visible = true; // Mostra a bolinha agora que o Dashboard apareceu!
        }

        // Cria a etiqueta de versão e copyright no canto da tela
        private void AdicionarMarcaDagua()
        {
            Label lblCopyright = new Label();
            lblCopyright.Text = "v0.1.0-BETA | © 2026 Victor Leonardi Design";
            lblCopyright.AutoSize = true;
            lblCopyright.BackColor = Color.Transparent;
            lblCopyright.ForeColor = Color.FromArgb(80, 255, 255, 255);
            lblCopyright.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            this.Controls.Add(lblCopyright);
            lblCopyright.Location = new Point(this.ClientSize.Width - lblCopyright.PreferredWidth - 10, this.ClientSize.Height - lblCopyright.PreferredHeight - 10);
            lblCopyright.BringToFront();
        }

        // Recorta os cantos de um painel para deixá-lo arredondado
        private void ArredondarPainel(Panel painel, int raio)
        {
            IntPtr ptr = CreateRoundRectRgn(0, 0, painel.Width, painel.Height, raio, raio);
            SetWindowRgn(painel.Handle, ptr, true);
        }

        // Atualiza as cores das barras e textos baseado no nível de bateria (0 a 3)
        private void AtualizarStatusBateria(byte nivel, bool conectado)
        {
            if (!carregamentoConcluido) return; // Não faz nada se o loading ainda estiver na tela

            if (!conectado)
            {
                lblNivelTexto.Text = "---";
                lblNivelTexto.ForeColor = Color.White;
                foreach (var bar in barrasControles) bar.BackColor = Color.FromArgb(45, 45, 45);
                return;
            }

            Color corNivel = Color.Gray;
            string textoNivel = "";

            // Define a cor e o texto baseado no retorno do hardware
            switch (nivel)
            {
                case 0: textoNivel = "Vazio:"; corNivel = Color.Red; break;
                case 1: textoNivel = "Baixo:"; corNivel = Color.Orange; break;
                case 2: textoNivel = "Médio:"; corNivel = Color.Yellow; break;
                case 3: textoNivel = "Cheio:"; corNivel = Color.LimeGreen; break;
            }

            lblNivelTexto.Text = textoNivel;
            lblNivelTexto.ForeColor = Color.White;

            // Pinta as barras de bateria proporcionalmente
            for (int i = 0; i < barrasControles.Length; i++)
            {
                if (i <= nivel)
                {
                    barrasControles[i].BackColor = corNivel;
                    barrasControles[i].BorderStyle = BorderStyle.None;
                }
                else
                {
                    barrasControles[i].BackColor = Color.FromArgb(45, 45, 45);
                    barrasControles[i].BorderStyle = BorderStyle.FixedSingle;
                }
            }
        }

        // Evento que roda continuamente (Timer) para monitorar o controle
        private void timer1_Tick(object sender, EventArgs e)
        {
            XINPUT_STATE state;
            int result = XInputGetState(0, out state); // Tenta ler o controle 1 (índice 0)
            bool agoraConectado = (result == 0);

            if (agoraConectado)
            {
                XINPUT_BATTERY_INFORMATION bInfo;
                XInputGetBatteryInformation(0, 0x00, out bInfo); // Pede info da bateria

                lblStatus.Text = "Status: Conectado";
                lblStatus.ForeColor = Color.LimeGreen;
                lblTempoUso.Text = (DateTime.Now - inicioUso).ToString(@"hh\:mm\:ss");

                AtualizarStatusBateria(bInfo.BatteryLevel, true);
                AtualizarIconeBandeja(bInfo.BatteryLevel, true);
            }
            else
            {
                lblStatus.Text = "Status: Desconectado";
                lblStatus.ForeColor = Color.Red;
                AtualizarStatusBateria(0, false);
                AtualizarIconeBandeja(0, false);
            }

            // Se o estado mudou (conectou ou desconectou), inicia a transição visual
            if (agoraConectado != estaConectado) { estaConectado = agoraConectado; timerTransicao.Start(); }
            VerificarAtalhoTeclado();
        }

        // Configura o ícone e o menu de clique direito perto do relógio do Windows
        private void ConfigurarBandeja()
        {
            notifyIcon1.Visible = true;
            notifyIcon1.Text = "Xbox 360 Battery Dashboard";
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.BackColor = Color.FromArgb(32, 32, 32);
            menu.ForeColor = Color.White;
            menu.ShowImageMargin = false;
            menu.Items.Add(new ToolStripMenuItem("Abrir Dashboard", null, (s, e) => RestaurarJanela()));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new ToolStripMenuItem("Sair", null, (s, e) => Application.Exit()));
            notifyIcon1.ContextMenuStrip = menu;
            notifyIcon1.MouseClick += (s, e) => { if (e.Button == MouseButtons.Left) RestaurarJanela(); };
        }

        // Converte bytes em um objeto de Ícone (.ico)
        private Icon CriarIcone(byte[]? bytes)
        {
            if (bytes == null) return this.Icon;
            using (MemoryStream ms = new MemoryStream(bytes)) { return new Icon(ms); }
        }

        // Muda o ícone da bandeja para mostrar o nível de bateria em tempo real
        private void AtualizarIconeBandeja(byte nivel, bool conectado)
        {
            try
            {
                if (!conectado) { notifyIcon1.Icon = this.Icon; return; }
                byte[] iconBytes = null;
                switch (nivel)
                {
                    case 0: iconBytes = Properties.Resources.battery_empty; break;
                    case 1: iconBytes = Properties.Resources.battery_low; break;
                    case 2: iconBytes = Properties.Resources.battery_medium; break;
                    case 3: iconBytes = Properties.Resources.battery_full; break;
                }
                if (iconBytes != null) notifyIcon1.Icon = CriarIcone(iconBytes);
            }
            catch { }
        }

        // Calcula a posição no canto inferior direito da tela (acima da barra de tarefas)
        private void PosicionarNoCanto()
        {
            Rectangle areaTrabalho = Screen.PrimaryScreen.WorkingArea;
            int posX = areaTrabalho.Right - this.Width - 620; // Ajuste manual de distância X
            int posY = areaTrabalho.Bottom - this.Height - 440; // Ajuste manual de distância Y
            this.SetDesktopLocation(posX, posY);
        }

        // Faz o app reaparecer na tela quando solicitado
        private void RestaurarJanela()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Opacity = 1;
            PosicionarNoCanto();
            this.Activate();
            // Truque para esconder da barra de tarefas logo após abrir (estilo dashboard)
            System.Threading.Tasks.Task.Delay(100).ContinueWith(t => {
                this.Invoke(new Action(() => this.ShowInTaskbar = false));
            });
        }

        // Controla o efeito de clarear/escurecer a imagem do controle suavemente
        private void TimerTransicao_Tick(object? sender, EventArgs e)
        {
            if (estaConectado && opacidade < 1f) opacidade += 0.05f;
            else if (!estaConectado && opacidade > 0.2f) opacidade -= 0.10f;
            opacidade = Math.Clamp(opacidade, 0.2f, 1f);
            picControle.Image = AplicarOpacidade(Properties.Resources.controle_on, opacidade);
            if (opacidade >= 1f || opacidade <= 0.2f) timerTransicao.Stop();
        }

        // Função técnica que redesenha uma imagem com um nível de transparência específico
        private Image? AplicarOpacidade(Image? img, float nivel)
        {
            if (img == null) return null;
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                var matrix = new ColorMatrix { Matrix33 = nivel }; // Matrix33 controla o canal Alpha (transparência)
                var imageAttr = new ImageAttributes();
                imageAttr.SetColorMatrix(matrix);
                g.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imageAttr);
            }
            return bmp;
        }

        // Monitora os botões do controle para abrir/fechar o app pelo atalho LT+RT+Back+Start
        private void VerificarAtalhoTeclado()
        {
            XINPUT_STATE state;
            if (XInputGetState(0, out state) == 0)
            {
                // Verifica se os gatilhos e os botões centrais estão pressionados ao mesmo tempo
                bool combo = (state.Gamepad.wButtons & 0x0030) == 0x0030 && state.Gamepad.bLeftTrigger > 200 && state.Gamepad.bRightTrigger > 200;
                if (combo && !botaoPressionado)
                {
                    if (this.Visible && this.WindowState == FormWindowState.Normal) { this.WindowState = FormWindowState.Minimized; this.Hide(); }
                    else RestaurarJanela();
                    botaoPressionado = true;
                }
                if (!combo) botaoPressionado = false;
            }
        }

        // Impede que o app feche de verdade ao clicar no "X", apenas minimiza para a bandeja
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) { e.Cancel = true; this.WindowState = FormWindowState.Minimized; this.Hide(); }
            base.OnFormClosing(e);
        }

        // Trata o comportamento de esconder da barra de tarefas ao minimizar
        protected override void OnResize(EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) { this.Hide(); this.ShowInTaskbar = false; }
            base.OnResize(e);
        }

        // Atalho rápido: dois cliques no ícone da bandeja abre o app
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) => RestaurarJanela();
        private void lblTempoUso_Click(object sender, EventArgs e) { }
    }
}