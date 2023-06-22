using Botzin.Classe;
using Newtonsoft.Json;

namespace Botzin.Service
{
    public class BuscarNoticiaService
    {
        private readonly string _url = "https://apinoticias.tedk.com.br/api/";
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<AgregadoNoticia> BuscarNoticiasAsync(string assunto, String data)
        {
            assunto = assunto.Replace(" ", "-");
            var queryParams = $"?q={assunto}&date={data}";
            var requestUrl = $"{_url}{queryParams}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var agregadoNoticia = JsonConvert.DeserializeObject<AgregadoNoticia>(content);
                    return agregadoNoticia;
                }
                else
                {
                    throw new Exception($"Falha ao acessar a API. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao acessar a API: {ex.Message}");
            }
        }
    }
}
