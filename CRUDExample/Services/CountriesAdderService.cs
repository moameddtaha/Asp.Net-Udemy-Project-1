using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepoCon;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesAdderService : ICountriesAdderService
    {
        //private field
        private readonly ICountriesRepository _countriesRepository;

        public CountriesAdderService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? request)
        {

            //Validation: CountryAddRequest parameter can't be null
            if (request == null)
            {
                throw new ArgumentNullException(nameof(CountryAddRequest));
            }

            // Validation: countryName can't be null
            if (request.CountryName == null)
            {
                throw new ArgumentException(nameof(CountryAddRequest.CountryName));
            }

            //Validation: CountryName can't be duplicate
            if (await _countriesRepository.GetCountryByCountryName(request.CountryName) != null)
            {
                throw new ArgumentException("Given country name already exists");
            }

            // Convert Object from CountryAddRequest to Country type
            Country country = request.ToCountry();

            //Generate CountryID
            country.CountryId = Guid.NewGuid();

            //Add country object into _countries
            await _countriesRepository.AddCountry(country);

            // TODO: Return and implement the stored procedure for adding countries

            //await _db.sp_InsertCountries(country);

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            List<Country> countries = await _countriesRepository.GetAllCountries();

            return countries
                .Select(country => country.ToCountryResponse())
                .ToList() ?? new List<CountryResponse>();

            // TODO: Return and implement the stored procedure for getting all countries

            //var countries = await _db.sp_GetAllCountries();
            //return countries.Select(temp => temp.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? CountryID)
        {
            if (CountryID == null)
            {
                return null;
            }
            Country? country_response_from_list = await _countriesRepository.GetCountryByCountryID(CountryID.Value);

            return country_response_from_list?.ToCountryResponse() ?? null;
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
                            CountryAddRequest countryAddRequest = new CountryAddRequest()
                            {
                                CountryName = countryName
                            };
                            //await _db.Countries.AddAsync(country);
                            //await _db.SaveChangesAsync();

                            // Add the country to the database
                            await AddCountry(countryAddRequest);

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
