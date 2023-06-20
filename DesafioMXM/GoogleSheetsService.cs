using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore;
using Newtonsoft.Json;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using static System.Net.WebRequestMethods;
using System.Text;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DesafioMXM
{
    public class GoogleSheetsService
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "ImoveisHelper";
        static readonly string sheet = "Página1";
        static readonly string SpreadsheetId = "API_KEY_SPREADSHEET";
        static SheetsService service;

        static public void Init(string nome, string email, string tel, string imovel, string mensagem)
        {
            GoogleCredential credential;

            using (var stream = new FileStream("Credentials/clients_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            CreateEntry(nome, email, tel, imovel, mensagem);
        }

        static public void CreateEntry(string nome, string email, string tel, string imovel, string mensagem)
        {
            var range = $"{sheet}!A:E";
            var valueRange = new ValueRange();

            var objectList = new List<object>() { nome, email, tel, imovel, mensagem };
            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();

            string webhookUrl = "https://imoveishelperfunctionsendgrid.azurewebsites.net";
            var formData = new
            {
                Nome = nome,
                Email = email,
                Telefone = tel,
                Imovel = imovel,
                Mensagem = mensagem
            };

            CallWebhook(webhookUrl, formData);
        }

        static public void CallWebhook(string webhookUrl, object formData)
        {
            using (var httpClient = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(formData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = httpClient.PostAsync(webhookUrl, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Webhook chamado com sucesso!");
                }
                else
                {
                    Console.WriteLine("Erro ao chamar o webhook: " + response.StatusCode);
                }
            }
        }

        
    }
}
