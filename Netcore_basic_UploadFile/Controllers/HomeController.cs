using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Netcore_basic_UploadFile.Data;
using Netcore_basic_UploadFile.Models;
using Netcore_basic_UploadFile.ViewModels;

namespace Netcore_basic_UploadFile.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext applicationDbContext,
            IWebHostEnvironment hostEn)
        {
            _logger = logger;
            _dbContext = applicationDbContext;
            _webHostEnvironment = hostEn;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _dbContext.Employees.ToListAsync();
            //IList<EmployeeView> emVm = new List<EmployeeViewModel>();
            return View(employees);
        }
        public IActionResult New()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UpLoadFileModel(model);
                if(uniqueFileName!=string.Empty)
                {
                    Employee em = new Employee {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        FullName = model.FirstName + " " + model.LastName,
                        Gender = model.Gender,
                        Age = model.Age,
                        Office = model.Office,
                        Position = model.Position,
                        Salary = model.Salary,
                        ProfilePicture = uniqueFileName,
                    };
                    _dbContext.Employees.Add(em);
                    int result = await _dbContext.SaveChangesAsync();
                    if(result>0)
                    {
                        int a = 1;
                    }
                    else
                    {
                        int a = 1;
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return BadRequest();
                }
                
            }
            return View();
        }
        public IActionResult Privacy()
        {
            
            return View();
        }

        private string UpLoadFileModel(EmployeeViewModel model)
        {
            string uniqueFileName = string.Empty;
            if(model.ProfileImage!=null)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = $"{Guid.NewGuid().ToString()}_{model.ProfileImage.FileName}";
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
