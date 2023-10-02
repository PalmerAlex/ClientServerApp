using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClientServerApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        [BindProperty]
        public string messageInput { get; set; }
        //This is the parameter for the form input
        //it is bound because the text box and variable share the same name (see the html view)

        public async Task<IActionResult> OnPostSendMessageAsync()
        {//This method triggers on form submission (once the Submit button is pressed)
            
            _logger.LogInformation($"Received message: {messageInput}");
            //This prints the submitted message to the server console

            return Page();
            //This refreshes the frontend display
        }
    }
}