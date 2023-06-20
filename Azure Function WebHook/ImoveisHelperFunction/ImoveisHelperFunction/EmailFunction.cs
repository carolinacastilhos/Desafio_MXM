using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

public static class EmailFunction
{
    [FunctionName("EmailFunction")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("EmailFunction triggered.");

        string requestBody = new StreamReader(req.Body).ReadToEnd();
        dynamic data = JsonConvert.DeserializeObject(requestBody);

        string nome = data?.nome;
        string email = data?.email;
        string tel = data?.tel;
        string imovel = data?.imovel;
        string mensagem = data?.mensagem;

        SendEmail(nome, email, tel, imovel, mensagem, log);

        return new OkResult();
    }

    static void SendEmail(string nome, string email, string tel, string imovel, string mensagem, ILogger log)
    {
        var sendGridApiKey = "API_KEY_SENDGRID";
        var senderEmail = "imoveishelper@gmail.com";
        var recipientEmail = "imoveishelper@gmail.com";

        var client = new SendGridClient(sendGridApiKey);

        var message = new SendGridMessage
        {
            From = new EmailAddress(senderEmail),
            Subject = "Novos dados do formulário",
            PlainTextContent = $"Nome: {nome}\nEmail: {email}\nTelefone: {tel}\nImóvel: {imovel}\nMensagem: {mensagem}",
            HtmlContent = $"<p><strong>Nome:</strong> {nome}</p><p><strong>Email:</strong> {email}</p><p><strong>Telefone:</strong> {tel}</p><p><strong>Imóvel:</strong> {imovel}</p><p><strong>Mensagem:</strong> {mensagem}</p>"
        };
        message.AddTo(new EmailAddress(recipientEmail));

        var response = client.SendEmailAsync(message).Result;

        log.LogInformation("E-mail enviado: " + response.StatusCode);
    }
}

