namespace Bateria_Controle_Xbox_360
{
    // A palavra 'partial' indica que esta é a metade visual da mesma classe Form1
    partial class Form1
    {
        // Gerenciador de componentes (necessário para o funcionamento interno do Windows Forms)
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpa os recursos que estão sendo usados (como memória e ícones) ao fechar o app.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// CONFIGURAÇÃO VISUAL: Aqui é onde o C# define posições, cores e fontes.
        /// Este método é gerado automaticamente pelo Designer do Visual Studio.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));

            // Instancia cada elemento da tela (Labels, Painéis, Imagens)
            timer1 = new System.Windows.Forms.Timer(components);
            notifyIcon1 = new NotifyIcon(components);
            lblP1 = new Label();
            lblStatus = new Label();
            lblNivel = new Label();
            lblNivelTexto = new Label();
            lblTempoUso = new Label();
            lblConectividade = new Label();
            linhaSeparadora = new Panel();
            bar1 = new Panel();
            bar2 = new Panel();
            bar3 = new Panel();
            bar4 = new Panel();
            picControle = new PictureBox();

            // Componentes adicionados manualmente para uso em tempo de execução
            lblInfoIcon = new Label();
            toolTipInfo = new System.Windows.Forms.ToolTip(components);

            ((System.ComponentModel.ISupportInitialize)picControle).BeginInit();
            SuspendLayout(); // Pausa o desenho da tela para configurar tudo de uma vez (evita lag)

            // --- Configuração do Timer de atualização ---
            timer1.Enabled = true;       // Ativa o timer assim que o app abre
            timer1.Interval = 1000;      // Define a taxa de atualização (1000ms = 1 segundo)
            timer1.Tick += timer1_Tick;  // Conecta o timer à função de lógica no outro arquivo

            // --- Label: "Controle 1:" ---
            lblP1.AutoSize = true;
            lblP1.Font = new Font("Segoe UI", 18F, FontStyle.Bold); // Fonte moderna e em negrito
            lblP1.Location = new Point(428, 50);                   // Coordenada X e Y na tela
            lblP1.Name = "lblP1";
            lblP1.Size = new Size(141, 32);
            lblP1.TabIndex = 1;
            lblP1.Text = "Controle 1:";

            // --- Label: "Status: Conectado" ---
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 12F);
            lblStatus.Location = new Point(430, 95);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(133, 21);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Status: Conectado";

            // --- Label: "Nivel da bateria:" ---
            lblNivel.AutoSize = true;
            lblNivel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblNivel.Location = new Point(429, 140);
            lblNivel.Name = "lblNivel";
            lblNivel.Size = new Size(202, 32);
            lblNivel.TabIndex = 3;
            lblNivel.Text = "Nivel da bateria:";

            // --- Label que mostra o texto (Vazio, Baixo, Médio, Cheio) ---
            lblNivelTexto.AutoSize = true;
            lblNivelTexto.Font = new Font("Segoe UI", 14F);
            lblNivelTexto.Location = new Point(430, 190);
            lblNivelTexto.Name = "lblNivelTexto";
            lblNivelTexto.Size = new Size(70, 25);
            lblNivelTexto.TabIndex = 4;
            lblNivelTexto.Text = "Médio:";

            // --- Label do Cronômetro (00:00:00) ---
            lblTempoUso.AutoSize = true;
            lblTempoUso.Font = new Font("Segoe UI", 16F);
            lblTempoUso.Location = new Point(430, 280);
            lblTempoUso.Name = "lblTempoUso";
            lblTempoUso.Size = new Size(95, 30);
            lblTempoUso.TabIndex = 6;
            lblTempoUso.Text = "00:00:00";
            lblTempoUso.Click += lblTempoUso_Click;

            // --- Label: "Tempo de uso" ---
            lblConectividade.AutoSize = true;
            lblConectividade.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblConectividade.Location = new Point(428, 240);
            lblConectividade.Name = "lblConectividade";
            lblConectividade.Size = new Size(174, 32);
            lblConectividade.TabIndex = 5;
            lblConectividade.Text = "Tempo de uso";

            // --- Linha Vertical Branca (Divisora entre Imagem e Texto) ---
            linhaSeparadora.BackColor = Color.White;
            linhaSeparadora.Location = new Point(400, 50);
            linhaSeparadora.Name = "linhaSeparadora";
            linhaSeparadora.Size = new Size(2, 300); // 2 de largura (fina) e 300 de altura
            linhaSeparadora.TabIndex = 0;

            // --- Blocos de bateria (bar1 até bar4) ---
            // Esses painéis funcionam como as "células" da bateria na interface
            bar1.BackColor = Color.LimeGreen;
            bar1.Location = new Point(508, 190);
            bar1.Size = new Size(40, 25);

            bar2.BackColor = Color.LimeGreen;
            bar2.Location = new Point(551, 190);
            bar2.Size = new Size(40, 25);

            bar3.BackColor = Color.LimeGreen;
            bar3.Location = new Point(594, 190);
            bar3.Size = new Size(40, 25);

            bar4.BackColor = Color.Gray; // Bar4 começa cinza para indicar que falta carregar
            bar4.Location = new Point(637, 190);
            bar4.Size = new Size(40, 25);

            // --- Imagem do Controle do Xbox ---
            picControle.Image = Properties.Resources.controle_off; // Imagem padrão (desligado)
            picControle.Location = new Point(30, 50);
            picControle.Name = "picControle";
            picControle.Size = new Size(350, 300);
            picControle.SizeMode = PictureBoxSizeMode.Zoom; // Faz a imagem caber no quadro sem distorcer
            picControle.TabIndex = 11;
            picControle.TabStop = false;

            // --- Configurações Gerais da Janela Principal (Form1) ---
            BackColor = Color.FromArgb(45, 45, 45); // Cor de fundo grafite escuro (Moderno)
            ClientSize = new Size(754, 397);        // Tamanho da janela

            // Adiciona todos os componentes criados acima dentro da janela
            Controls.Add(linhaSeparadora);
            Controls.Add(lblP1);
            Controls.Add(lblStatus);
            Controls.Add(lblNivel);
            Controls.Add(lblNivelTexto);
            Controls.Add(lblConectividade);
            Controls.Add(lblTempoUso);
            Controls.Add(bar1);
            Controls.Add(bar2);
            Controls.Add(bar3);
            Controls.Add(bar4);
            Controls.Add(picControle);

            ForeColor = Color.White; // Define que todos os textos serão brancos por padrão
            FormBorderStyle = FormBorderStyle.FixedSingle; // Impede o usuário de esticar a janela
            Icon = (Icon)resources.GetObject("$this.Icon"); // Carrega o ícone do projeto
            Name = "Form1";
            Text = "Xbox 360 Battery Dashboard (v0.1.0-BETA)"; // Título da barra superior

            ((System.ComponentModel.ISupportInitialize)picControle).EndInit();
            ResumeLayout(false); // Retoma o desenho da tela
            PerformLayout();      // Organiza os itens conforme as regras definidas
        }

        // --- LISTA DE COMPONENTES ---
        // Aqui o código apenas reserva o espaço na memória para cada item
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Label lblP1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblNivel;
        private System.Windows.Forms.Label lblNivelTexto;
        private System.Windows.Forms.Label lblConectividade;
        private System.Windows.Forms.Label lblTempoUso;
        private System.Windows.Forms.Panel bar1;
        private System.Windows.Forms.Panel bar2;
        private System.Windows.Forms.Panel bar3;
        private System.Windows.Forms.Panel bar4;
        private System.Windows.Forms.PictureBox picControle;
        private Panel linhaSeparadora;
        // Componentes adicionados manualmente
        private System.Windows.Forms.Label lblInfoIcon;
        private System.Windows.Forms.ToolTip toolTipInfo;
    }
}