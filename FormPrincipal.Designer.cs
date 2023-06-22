namespace Botzin
{
    partial class FormPrincipal
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            groupBox1 = new GroupBox();
            btnEnviarMensagem = new Button();
            button1 = new Button();
            txtMensagem = new TextBox();
            btnSetupObserver = new Button();
            btnCarregar = new Button();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Dock = DockStyle.Fill;
            webView21.Location = new Point(0, 0);
            webView21.Name = "webView21";
            webView21.Size = new Size(1002, 712);
            webView21.TabIndex = 0;
            webView21.ZoomFactor = 1D;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnEnviarMensagem);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(txtMensagem);
            groupBox1.Controls.Add(btnSetupObserver);
            groupBox1.Controls.Add(btnCarregar);
            groupBox1.Dock = DockStyle.Bottom;
            groupBox1.Location = new Point(0, 648);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1002, 64);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Opções";
            // 
            // btnEnviarMensagem
            // 
            btnEnviarMensagem.Location = new Point(758, 27);
            btnEnviarMensagem.Name = "btnEnviarMensagem";
            btnEnviarMensagem.Size = new Size(189, 34);
            btnEnviarMensagem.TabIndex = 6;
            btnEnviarMensagem.Text = "Enviar Mensagem";
            btnEnviarMensagem.UseVisualStyleBackColor = true;
            btnEnviarMensagem.Click += btnEnviarMensagem_Click;
            // 
            // button1
            // 
            button1.Location = new Point(557, 27);
            button1.Name = "button1";
            button1.Size = new Size(171, 34);
            button1.TabIndex = 5;
            button1.Text = "Setar Mensagem";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // txtMensagem
            // 
            txtMensagem.Location = new Point(309, 27);
            txtMensagem.Name = "txtMensagem";
            txtMensagem.Size = new Size(242, 31);
            txtMensagem.TabIndex = 4;
            // 
            // btnSetupObserver
            // 
            btnSetupObserver.Location = new Point(130, 25);
            btnSetupObserver.Name = "btnSetupObserver";
            btnSetupObserver.Size = new Size(173, 33);
            btnSetupObserver.TabIndex = 3;
            btnSetupObserver.Text = "Iniciar Observer";
            btnSetupObserver.UseVisualStyleBackColor = true;
            btnSetupObserver.Click += btnSetupObserver_Click;
            // 
            // btnCarregar
            // 
            btnCarregar.Location = new Point(12, 24);
            btnCarregar.Name = "btnCarregar";
            btnCarregar.Size = new Size(112, 34);
            btnCarregar.TabIndex = 2;
            btnCarregar.Text = "Carregar";
            btnCarregar.UseVisualStyleBackColor = true;
            btnCarregar.Click += btnCarregar_Click;
            // 
            // FormPrincipal
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1002, 712);
            Controls.Add(groupBox1);
            Controls.Add(webView21);
            Name = "FormPrincipal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            Load += FormPrincipal_Load;
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private GroupBox groupBox1;
        private Button btnCarregar;
        private Button btnSetupObserver;
        private Button btnEnviarMensagem;
        private Button button1;
        private TextBox txtMensagem;
    }
}