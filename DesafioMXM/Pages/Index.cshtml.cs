using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DesafioMXM.Pages
{
    public class LoginModel : PageModel
    {
        public void OnPost(string nome, string email, string tel, string imovel, string mensagem)
        {
            GoogleSheetsService.Init(nome, email, tel, imovel, mensagem);
        }
    }
    
}
