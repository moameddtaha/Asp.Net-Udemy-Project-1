using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepoCon;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesUploaderService : ICountriesUploaderService
    {
        //private field
        private readonly ICountriesRepository _countriesRepository;

        public CountriesUploaderService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);

            int countriesInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {

                // Check if the workbook contains the "Countries" worksheet
                ExcelWorksheet? worksheet = excelPackage.Workbook.Worksheets["Countries"];

                if (worksheet == null)
                {
                    throw new ArgumentException("The Excel file does not contain a worksheet named 'Countries'.");
                }

                int rowCount = worksheet.Dimension?.Rows ?? 0; // Use null conditional to avoid exceptions

                if (rowCount < 2) // Assuming the first row is headers
                {
                    throw new ArgumentException("The 'Countries' worksheet does not contain any data.");
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;

                        // Check if the country already exists in the database
                        if (_countriesRepository.GetCountryByCountryName(countryName) == null)
                        {
                            
                            Country country = new Country()
                            {
                                CountryName = countryName
                            };

                            // Add the country to the database
                            await _countriesRepository.AddCountry(country);

                            countriesInserted++;
                        }
                        else
                        {
                            throw new ArgumentException("The data already exists");
                        }
                    }
                }
            }
            return countriesInserted;
        }
    }
}
