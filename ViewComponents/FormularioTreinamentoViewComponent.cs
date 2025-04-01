using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Projeto_Trainee_Hub.ViewComponents
{
    [ViewComponent(Name = "FormularioTreinamento")]
    public class FormularioTreinamentoViewComponent : ViewComponent
    {
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var treinamento = new Models.Treinamento();
            return View(treinamento);
        }

    }


    
}