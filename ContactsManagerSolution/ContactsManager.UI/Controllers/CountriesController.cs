using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesAdderService _countriesAdderService;
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ICountriesUploaderService _countriesUploaderService;

        public CountriesController(ICountriesAdderService countriesAdderService, ICountriesGetterService countriesGetterService, ICountriesUploaderService countriesUploaderService)
        {
            _countriesAdderService = countriesAdderService;
            _countriesGetterService = countriesGetterService;
            _countriesUploaderService = countriesUploaderService;
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {

            if (excelFile == null || excelFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select an Excel (.xlsx) file. \n";
                return View();
            }

            // Validate file extension
            var allowedExtensions = new[] { ".xlsx" };
            var fileExtension = Path.GetExtension(excelFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ViewBag.ErrorMessage = "Unsupported file. \".xlsx\" file is expected \n";
                return View();
            }

            // Validate file size (example: limit to 5 MB)
            const long maxFileSize = 5 * 1024 * 1024; // 5 MB
            if (excelFile.Length > maxFileSize)
            {
                ViewBag.ErrorMessage = "File size exceeds the limit of 5 MB \n";
                return View();
            }

            try
            {
                int countriesCountInserted = await _countriesUploaderService.UploadCountriesFromExcelFile(excelFile);

                ViewBag.Message = $"{countriesCountInserted} countries uploaded";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View();
        }
    }
}
