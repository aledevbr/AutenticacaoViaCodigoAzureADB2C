using System;

namespace AplicativoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Teste de autenticação no Azure AD B2C");

            string authorizationHeader =
                new Autenticacao()
                .BuscarAuthorizationHeaderComUsuarioSenha().Result;

            Console.WriteLine("==================================");
            Console.WriteLine("Header retornado pela autenticação");
            Console.WriteLine("==================================");
            
            Console.WriteLine(authorizationHeader);
            
            Console.WriteLine("==================================");

            Console.WriteLine("\n\n[--- Aperte ENTER para chamar a API ---]");
            Console.ReadLine();

            // Realiza a chamada da API
            WebApi.ChamarWebApi(authorizationHeader);

            Console.WriteLine("[--- FIM ---]");

            // Aguarda um ENTER para sair do console
            Console.ReadLine();
        }
    }
}
