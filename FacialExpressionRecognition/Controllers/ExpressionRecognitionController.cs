using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FacialExpressionRecognitionML.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Hosting;
using FacialExpressionRecognition.Models;
using Microsoft.CodeAnalysis.Operations;


namespace FacialExpressionRecognition.Controllers
{
    public class ExpressionRecognitionController : Controller
    {

        private readonly IWebHostEnvironment environment;
        public ExpressionRecognitionController(IWebHostEnvironment _environment)
        {
            environment = _environment;
        }

        [HttpGet]
        public IActionResult ExpressionRecognition()
        {
            return View();
        }

       public IActionResult About()
        {
            return View();
        }
    
        [HttpPost]
        public ActionResult ExpressionRecognition(ModelInput input, IFormFile file)
        {

            if (file == null )
            {
                ViewData["Error"] = "Nie wybrano zdjecia";
                return View();
            }

            var fileName = Path.GetTempFileName();
            var filePath  = Path.Combine(environment.WebRootPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);              
            }
                
            input.ImageSource = filePath;

            var files = HttpContext.Request.Form.Files;
            using (var fs = files[0].OpenReadStream())
            {
                using var ms = new MemoryStream();
                fs.CopyTo(ms);

                ImageManager.image = ms.ToArray();
            }
     
            var stockPredictions = ConsumeModel.Predict(input);


            ExpressionResult expression = new ExpressionResult();
            for (int i =0; i < stockPredictions.Score.Length; i++)
            {
                expression.Score.Add(stockPredictions.Score[i]);
            }

            ExpressionResult.GetExpressionName(expression);

            ViewBag.Result = stockPredictions;     
            return View(expression);
       
           
        }
      
    }
}



