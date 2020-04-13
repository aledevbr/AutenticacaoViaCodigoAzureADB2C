using Microsoft.Identity.Client; // * PublicClientApplicationBuilder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security; // * SecureString()
using System.Text;
using System.Threading.Tasks;

namespace AplicativoConsole
{
    public class Autenticacao
    {
        public async Task<string> BuscarAuthorizationHeaderComUsuarioSenha()
        {
            // o nome do usuário também poderia ser
            // postuser@domain.com
            // pois cadastramos os dois
            string usuario = "postuser";
            // a senha que foi gerada ou inserida no Azure
            string senha = "Ruqu0319";

            // o locatário também pode ser o ID
            // exemplo: f1a92ca1-553c-4a1d-a00c-f7ab3c1445c7
            string locatario = "seudominio.onmicrosoft.com";
            // ID do aplicativo (cliente) => post-ConsoleApp
            string idCliente = "2b2ce9d4-b4a1-4cee-a0c1-46305f5e0a14";
            // Fluxo do usuário para a autenticaçãp
            string policySignUpSignIn = "B2C_1_post_ropc";

            // Interpolação de strings para utilização na autenticação
            string authorityBase = $"https://seudominio.b2clogin.com/tfp/{locatario}/";
            string authority = $"{authorityBase}{policySignUpSignIn}";

            // O escopo é referente a API exposta no aplicativo post-ApiWeb
            // pois é quem efetivamente iremos acessar
            string[] scopes = new string[]
            {
                // aqui é utilizado o ID do aplicativo (cliente) => post-ApiWeb
                "https://seudominio.onmicrosoft.com/e97f0bde-d845-4683-9717-cfc234c82659/post.sample"
            };

            var application = PublicClientApplicationBuilder.Create(idCliente)
                .WithB2CAuthority(authority)
                .Build();
            IEnumerable<IAccount> accounts = await application.GetAccountsAsync();

            string authorizationHeader = string.Empty;
            AuthenticationResult authenticationResult = null;
            if (accounts.Any())
            {
                authenticationResult =
                    await application.AcquireTokenSilent(scopes,
                                                         accounts.FirstOrDefault())
                    .ExecuteAsync();
            }
            else
            {
                try
                {
                    // cria uma forma segura de transmitir a senha
                    // com o objeto do tipo SecureString
                    var senhaSegura = new SecureString();
                    foreach (char c in senha)
                        senhaSegura.AppendChar(c);

                    // esse próximo passo eu não consegui executar com o await
                    // na execução ele parava e saia do console sem acusar 
                    // nenhum tipo de erro.
                    // por isso utilizo o .Wait() e depois pego o resultado
                    var taskAcquire = application.AcquireTokenByUsernamePassword(
                        scopes,
                        usuario,
                        senhaSegura).ExecuteAsync();
                    // não determinei nenhum timeout para o .Wait()
                    taskAcquire.Wait();
                    authenticationResult = taskAcquire.Result;

                    // o comando abaixo recupera o access token 
                    // e insere o "Bearer" no início, já deixando
                    // pronto para utilização nas chamadas de API
                    authorizationHeader =
                        authenticationResult.CreateAuthorizationHeader();

                }
                catch (MsalException mex)
                {
                    // Caso ocorra algum erro na autenticação
                    // deve ser tratado aqui
                    Console.WriteLine(mex);
                }
                catch (Exception ex)
                {
                    // Qualquer outro tipo de erro
                    Console.WriteLine(ex);
                }
            }

            // capturas possíveis (algumas)
            // --------------------------------------------------
            // caso queira recuperar apenas o access token
            string accessToken = authenticationResult.AccessToken;
            // caso queira recuperar apenas o ID token
            string idToken = authenticationResult.IdToken;
            // caso queira recuperar a hora em que o token expira
            DateTimeOffset expiresOn = authenticationResult.ExpiresOn;
            // --------------------------------------------------

            // retorna o header com o Bearer
            return authorizationHeader;
        }
    }
}
