using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http; // * HttpClient
using System.Net.Http.Headers; // * MediaTypeWithQualityHeaderValue()
using System.Text;
using System.Threading.Tasks;

namespace AplicativoConsole
{
    public class WebApi
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static void ChamarWebApi(string AuthorizationHeader)
        {
            // a URI do nosso projeto está a padrão, na porta 5000
            httpClient.BaseAddress = new Uri("http://localhost:5000/");

            // definimos o header para aceitar um json como reposta
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // aqui definimos o Header de requisição com
            // o token retornado do passo anterior
            // (será passado como parâmetro neste método)
            httpClient.DefaultRequestHeaders
                .Add("Authorization", AuthorizationHeader);

            // Faremos uma requisição da API que está em execução
            // A action acionada será a WeatherForecast
            HttpResponseMessage response =
                httpClient.GetAsync("WeatherForecast/")
                .Result;

            Console.WriteLine($"Resultado da chamada: {response.StatusCode}");

            // Verifica se foi uma resposta de sucesso da API
            if (response.IsSuccessStatusCode)
            {
                // Imprime o conteúdo da resposta
                var conteudo = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine("==================================");
                Console.WriteLine(conteudo);
                Console.WriteLine("==================================");
            }
        }
    }
}
