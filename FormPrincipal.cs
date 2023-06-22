using Botzin.Classe;
using Botzin.Enum;
using Botzin.Service;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using HAP = HtmlAgilityPack;

namespace Botzin
{
    public partial class FormPrincipal : Form
    {
        private Config Config { get; set; }
        private List<Mensagem> Mensagens = new List<Mensagem>();
        private SemaphoreSlim SemaphoreProcessamentoMensagensPendentes = new SemaphoreSlim(1, 1);
        private SemaphoreSlim SemaphorewebMessageReceived = new SemaphoreSlim(1, 1);
        private SemaphoreSlim SemaphoreSetarMensagem = new SemaphoreSlim(1, 1);
        private SemaphoreSlim SemaphoreEnviarMensagem = new SemaphoreSlim(1, 1);
        private StatusDoBot statusDoBot = StatusDoBot.Ligado;

        private const String QUEBRA = "\r\n";
        public FormPrincipal()
        {
            InitializeComponent();
        }

        private async Task AgendarProcessamentoMensagensPendentes()
        {
            while (true)
            {
                await SemaphoreProcessamentoMensagensPendentes.WaitAsync();

                await Task.Run(async () =>
                {
                    try
                    {
                        var MensagensPendentes = Mensagens.Where(x => x.Status == StatusMensagem.PendenteDeResposta).ToList();

                        foreach (var Mensagem in MensagensPendentes)
                        {
                            switch (Mensagem.Acao)
                            {
                                case AcaoBOT.AcaoDesconhecida:
                                    await ResponderAcaoDesconhecida(Mensagem);
                                    break;
                                case AcaoBOT.Echo:
                                    await ResponderEcho(Mensagem);
                                    break;
                                case AcaoBOT.GPT:
                                    await ResponderGPT(Mensagem);
                                    break;
                                case AcaoBOT.Desligar:
                                    await DesligarBot(Mensagem);
                                    break;
                                case AcaoBOT.Ligar:
                                    await LigarBot(Mensagem);
                                    break;
                                case AcaoBOT.Noticia:
                                    await BuscarNoticia(Mensagem);
                                    break;
                                default:
                                    break;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        String log = ex.Message;

                    }
                    finally
                    {
                        await Task.Delay(2000);
                        SemaphoreProcessamentoMensagensPendentes.Release();
                    }
                });
            }
        }

        private async Task EnviarMensagemNoticiaNaoEncontrada(Mensagem mensagem)
        {
            var TextoMensagem = $"Não foi encontrada nenhuma notícia recente para a palavra chave \"{mensagem.Comando}\"";

            await SetarMensagem(TextoMensagem, true);
            await Task.Delay(1000);
            await EnviarMensagem(true);
            mensagem.Status = StatusMensagem.Respondida;
        }

        private async Task BuscarNoticia(Mensagem mensagem)
        {
            var AgregadoNoticia = await new BuscarNoticiaService().BuscarNoticiasAsync(mensagem.Comando, DateTime.Now.ToString("dd/MM/yyyy"));

            if (AgregadoNoticia != null)
            {
                if (AgregadoNoticia?.list?.Count > 0)
                {
                    List<Noticias> noticias;
                    noticias = AgregadoNoticia.list.Count() >= 5 ? AgregadoNoticia.list.Take(5).ToList() : AgregadoNoticia.list.Take(AgregadoNoticia.list.Count).ToList();
                    String Mensagem = String.Empty;
                    foreach (var item in noticias)
                    {
                        var DataNoticia = DateTime.ParseExact(item.date, "dd/MM/yyyy",
                       System.Globalization.CultureInfo.InvariantCulture);

                        if (DataNoticia > DateTime.Now.AddDays(-7))
                        {
                            Mensagem += item.datetime + " - " + "*" + item.title + "*" + " - " + item.link + Environment.NewLine;
                        }
                    }

                    if (!String.IsNullOrEmpty(Mensagem))
                    {
                        await SetarMensagem(Mensagem, true);
                        await Task.Delay(1000);
                        await EnviarMensagem(true);
                        mensagem.Status = StatusMensagem.Respondida;
                    }
                    else
                    {
                        await EnviarMensagemNoticiaNaoEncontrada(mensagem);
                    }
                }
                else
                {
                    await EnviarMensagemNoticiaNaoEncontrada(mensagem);
                }
            }
        }

        private async Task LigarBot(Mensagem mensagem)
        {
            if (this.statusDoBot == StatusDoBot.Desligado)
            {
                await SetarMensagem("Esse Bot muito louco voltou pra aprontar altas confusões!", true);
                await Task.Delay(1000);
                await EnviarMensagem(true);
                mensagem.Status = StatusMensagem.Respondida;
                this.statusDoBot = StatusDoBot.Ligado;
            }
            else
            {
                await SetarMensagem("Não tem como ligar novamente o que já está ligado, melhore.", true);
                await Task.Delay(1000);
                await EnviarMensagem(true);
                mensagem.Status = StatusMensagem.Respondida;
            }
        }

        private async Task DesligarBot(Mensagem mensagem)
        {
            if (this.statusDoBot == StatusDoBot.Ligado)
            {
                await SetarMensagem("Fui de soneca, bjus.", true);
                await Task.Delay(1000);
                await EnviarMensagem(true);
                mensagem.Status = StatusMensagem.Respondida;
                this.statusDoBot = StatusDoBot.Desligado;
            }
            else
            {
                await SetarMensagem("Não tem como desligar novamente o que já está desligado, melhore.", true);
                await Task.Delay(1000);
                await EnviarMensagem(true);
                mensagem.Status = StatusMensagem.Respondida;
            }
        }

        private async Task ResponderGPT(Mensagem mensagem)
        {
            var Resposta = await PerguntarAoChatGPT(mensagem);
            await SetarMensagem(Resposta, true);
            await Task.Delay(1000);
            await EnviarMensagem(true);
            mensagem.Status = StatusMensagem.Respondida;
        }

        private async Task ResponderEcho(Mensagem mensagem)
        {
            await SetarMensagem(mensagem.Comando, true);
            await Task.Delay(1000);
            await EnviarMensagem(true);
            mensagem.Status = StatusMensagem.Respondida;
        }

        private Task ResponderAcaoDesconhecida(Mensagem mensagem)
        {
            return Task.CompletedTask;
        }

        private async Task CarregarWebView()
        {
            await webView21.EnsureCoreWebView2Async(null);
            webView21.WebMessageReceived += WebView21_WebMessageReceived;
        }

        private async void FormPrincipal_Load(object sender, EventArgs e)
        {
            await CarregarWebView();
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            if (!File.Exists(configPath))
            {
                MessageBox.Show("Arquivo de configuração não encontrado");
                this.Close();
            }

            var configJson = File.ReadAllText(configPath);
            Config = JsonConvert.DeserializeObject<Config>(configJson);
            this.WindowState = FormWindowState.Maximized;
            await AgendarProcessamentoMensagensPendentes();

        }

        private void btnCarregar_Click(object sender, EventArgs e)
        {
            webView21.Source = new Uri("https://web.whatsapp.com");
        }

        private async void WebView21_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (e.TryGetWebMessageAsString() == "TemMensagemNova")
            {
                await SemaphorewebMessageReceived.WaitAsync();
                try
                {
                    await TratarMensagens();
                }
                finally
                {
                    SemaphorewebMessageReceived.Release();
                }
            }
        }

        private async Task SetupObserver()
        {
            var Seletor = $"document.querySelector(\"{Config.SeletorPrincipal}\").innerHTML";
            var Conteudo = await webView21.CoreWebView2.ExecuteScriptAsync(Seletor);

            if (String.IsNullOrEmpty(Conteudo) || Conteudo.ToUpper() == "NULL")
            {
                MessageBox.Show("Informação para observar não foi encontrada, verifique se está na aba correta ou se a janela já carregou.");
                return;
            }

            string script = $@"
            (function() {{
                const targetNode = document.querySelector('{Config.SeletorPrincipal}');

                const observerOptions = {{
                    childList: true,
                    subtree: true,
                }};

                const observer = new MutationObserver((mutations) => {{
                    mutations.forEach((mutation) => {{
                        if (mutation.type === 'childList') {{
                            window.chrome.webview.postMessage('TemMensagemNova');
                        }}
                    }});
                }});

                observer.observe(targetNode, observerOptions);
            }})();
            ";

            await webView21.CoreWebView2.ExecuteScriptAsync(script);
        }

        private async void btnSetupObserver_Click(object sender, EventArgs e)
        {
            await SetupObserver();
        }

        public async Task<String> PerguntarAoChatGPT(Mensagem mensagem)
        {
            return await new OpenAIService(Config.OpenAPIApiKey).PerguntarAoChatGPT(mensagem.Comando, mensagem.Personagem);
        }

        private async Task EnviarMensagem(Boolean OutraThread = false)
        {
            await SemaphoreEnviarMensagem.WaitAsync();
            try
            {
                string script = @"
                (function() {
                    try {
                        let botaoEnviar = document.querySelector('#main > footer > div._2lSWV._3cjY2.copyable-area > div > span:nth-child(2) > div > div._1VZX7 > div._2xy_p._3XKXx > button');
                        botaoEnviar.click();
                    } catch (error) {
                        console.error('Erro ao executar consulta EnviarMensagem:', error);
                        return null;
                    }
                })();";

                if (OutraThread)
                {
                    this.Invoke((MethodInvoker)async delegate
                    {
                        await webView21.CoreWebView2.ExecuteScriptAsync(script);
                    });
                }
                else
                {
                    await webView21.CoreWebView2.ExecuteScriptAsync(script);
                }
            }
            finally
            {
                SemaphoreEnviarMensagem.Release();
            }
        }

        private async Task SetarMensagem(string message, Boolean OutraThread = false)
        {
            await SemaphoreSetarMensagem.WaitAsync();

            try
            {
                var escapedMessage = JavaScriptEncoder.Default.Encode(message);
                string script = @"
                function SetarMensagem(text) {
                    try {
                        const dataTransfer = new DataTransfer();
                        dataTransfer.setData('text', text);
                        const event = new ClipboardEvent('paste', {
                                clipboardData: dataTransfer,
                                bubbles: true
                            });
                    let el = document.querySelector('#main .copyable-area [contenteditable=""true""][role=""textbox""]');
                    el.dispatchEvent(event);
                    } catch (error) {
                        console.error('Erro ao executar consulta SetarMensagem:', error);
                        return null;
                    }
                }

                SetarMensagem('" + escapedMessage + "');";

                if (OutraThread)
                {
                    this.Invoke((MethodInvoker)async delegate
                    {
                        await webView21.CoreWebView2.ExecuteScriptAsync(script);
                    });
                }
                else
                {
                    await webView21.CoreWebView2.ExecuteScriptAsync(script);
                }
            }
            finally
            {
                SemaphoreSetarMensagem.Release();
            }
        }


        private async Task TratarMensagens()
        {
            //Pega a DIV Parent da DIVs de mensagens
            var script = $"document.querySelector(\"{Config.SeletorPrincipal}\").innerHTML";
            var Conteudo = await webView21.CoreWebView2.ExecuteScriptAsync(script);
            string htmlContent = JsonConvert.DeserializeObject<string>(Conteudo);

            HAP.HtmlDocument doc = new HAP.HtmlDocument();
            doc.LoadHtml(htmlContent);

            //Pega as divs que tem as mensagens
            var messageRows = doc.DocumentNode.SelectNodes(Config.SeletorLinhasMensagem);

            foreach (var row in messageRows)
            {
                var messageSpan = row.SelectSingleNode(Config.SeletorMensagem);
                var messageIdDiv = row.SelectSingleNode(Config.SeletorDivIdMensagem);
                var messageId = messageIdDiv?.GetAttributeValue("data-id", null);

                if (messageSpan != null && messageId != null)
                {
                    //Desconsidera mensagens de resposta do BOT
                    if (messageId.Contains(Config.NumeroDoBOT))
                    {
                        continue;
                    }

                    string messageText = messageSpan.InnerText.Trim();

                    //Se a mensagem não existe na lista, adiciona
                    if (Mensagens.Where(x => x.Id == messageId).FirstOrDefault() == null && messageText.Contains("!bot"))
                    {
                        String Personagem = String.Empty;

                        if (messageText.ToLower().Contains("personagem") && messageText.ToLower().Contains("nintendista"))
                        {
                            Personagem = "nintendista";
                        }
                        Mensagens.Add(new Mensagem { Id = messageId, Texto = messageText, Status = StatusMensagem.PendenteDeResposta, Acao = DefinirAcao(messageText), Comando = DefinirComando(messageText), Personagem = Personagem });
                    }
                }
            }
        }

        private string DefinirComando(string messageText)
        {
            Regex regex = new Regex(@"""([^""]*)""");
            Match match = regex.Match(messageText);
            String Comando = String.Empty;
            if (match.Success)
            {
                Comando = match.Groups[1].Value;
            }

            return Comando;
        }

        static string[] DefinirComandoEParametros(string messageText)
        {
            string padraoAcao = @"!bot (\S+)";
            Regex regexAcao = new Regex(padraoAcao);
            Match matchAcao = regexAcao.Match(messageText);

            if (matchAcao.Success)
            {
                string acao = matchAcao.Groups[1].Value;
                string comandoSemAcao = messageText.Substring(matchAcao.Groups[0].Length);
                string padraoParametros = @"(?<=\s)(?:""[^""]*(?:'[^']*)*[^""]*""|\S+)(?=\s|$)";
                Regex regexParametros = new Regex(padraoParametros);
                MatchCollection matchesParametros = regexParametros.Matches(comandoSemAcao);

                List<string> elementos = new List<string> { acao };

                foreach (Match match in matchesParametros)
                {
                    elementos.Add(match.Value.Trim());
                }

                return elementos.ToArray();
            }
            else
            {
                throw new ArgumentException("Comando inválido.");
            }
        }

        private AcaoBOT DefinirAcao(string messageText)
        {
            var Resultado = AcaoBOT.AcaoDesconhecida;

            if (messageText.ToLower().Contains("echo"))
            {
                Resultado = AcaoBOT.Echo;
            }
            else if (messageText.ToLower().Contains("gpt"))
            {
                Resultado = AcaoBOT.GPT;
            }
            else if (messageText.ToLower().Contains("desligar"))
            {
                Resultado = AcaoBOT.Desligar;
            }
            else if (messageText.ToLower().Contains("ligar"))
            {
                Resultado = AcaoBOT.Ligar;
            }
            else if (messageText.ToLower().Contains("noticia") || messageText.ToLower().Contains("notícia"))
            {
                Resultado = AcaoBOT.Noticia;
            }
            else
            {
                Resultado = AcaoBOT.AcaoDesconhecida;
            }

            return Resultado;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await SetarMensagem(txtMensagem.Text);
        }

        private async void btnEnviarMensagem_Click(object sender, EventArgs e)
        {
            await EnviarMensagem();
        }
    }
}