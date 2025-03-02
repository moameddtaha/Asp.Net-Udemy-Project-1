using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Globalization;
using OfficeOpenXml;
using RepoCon;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using Exceptions;

namespace Services
{
    public class PersonsGetterService : IPersonsGetterService
    {
        //Private field
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsGetterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public virtual async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsService");

            // SELECT * FROM Persons

            IList<Person> persons = await _personsRepository.GetAllPersons();

            return persons
                .Select(temp => temp.ToPersonResponse())
                .ToList();
        }

        public virtual async Task<PersonResponse?> GetPersonbyPersonID(Guid? personID)
        {
            if (personID == null)
            {
                return null;
            }

            Person? person = await _personsRepository.GetPersonbyPersonID(personID.Value);

            if (person == null)
            {
                return null;
            }

            return person.ToPersonResponse();
        }

        public virtual async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsService");

            List<Person> persons;

            using (Operation.Time("Time for Filtered Persons from Database"))
            {

                persons = searchBy switch
                {
                    nameof(PersonResponse.PersonName) =>
                        await _personsRepository.GetFilteredPersons(p =>
                            !string.IsNullOrEmpty(p.PersonName) &&
                            !string.IsNullOrEmpty(searchString) &&
                            p.PersonName.Contains(searchString)),

                    nameof(PersonResponse.Email) =>
                        await _personsRepository.GetFilteredPersons(p =>
                           !string.IsNullOrEmpty(p.Email) &&
                           !string.IsNullOrEmpty(searchString) &&
                           p.Email.Contains(searchString)),

                    nameof(PersonResponse.DateOfBirth) =>
                        await _personsRepository.GetFilteredPersons(p =>
                            p.DateOfBirth.HasValue &&
                            !string.IsNullOrEmpty(searchString) &&
                            p.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),

                    nameof(PersonResponse.Gender) =>
                        await _personsRepository.GetFilteredPersons(p =>
                            !string.IsNullOrEmpty(p.Gender) &&
                            !string.IsNullOrEmpty(searchString) &&
                            p.Gender.Contains(searchString)),

                    nameof(PersonResponse.CountryName) =>
                        await _personsRepository.GetFilteredPersons(p =>
                           p.Country != null &&
                           p.Country.CountryName != null &&
                           !string.IsNullOrEmpty(searchString) &&
                           p.Country.CountryName.Contains(searchString)),

                    nameof(PersonResponse.Address) =>
                        await _personsRepository.GetFilteredPersons(p =>
                            !string.IsNullOrEmpty(p.Address) &&
                            !string.IsNullOrEmpty(searchString) &&
                            p.Address.Contains(searchString)),

                    _ => await _personsRepository.GetAllPersons()
                };
            } // End of "using block" of serilog timings

            _diagnosticContext.Set("Persons", persons);

            return persons.Select(p => p.ToPersonResponse()).ToList();
        }

        public virtual async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

            using (CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration, leaveOpen: true))
            {
                // PersonName, Email, etc (comma is added automatically)
                csvWriter.WriteField(nameof(PersonResponse.PersonName));
                csvWriter.WriteField(nameof(PersonResponse.Email));
                csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
                csvWriter.WriteField(nameof(PersonResponse.Age));
                csvWriter.WriteField(nameof(PersonResponse.CountryName));
                csvWriter.WriteField(nameof(PersonResponse.Address));
                csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetter));
                csvWriter.NextRecord(); // next line

                List<PersonResponse> persons = await GetAllPersons();

                foreach (var person in persons)
                {
                    csvWriter.WriteField(person.PersonName);
                    csvWriter.WriteField(person.Email);
                    csvWriter.WriteField(person.DateOfBirth != null ? person.DateOfBirth.Value.ToString("dd-MM-yyyy") : "");
                    csvWriter.WriteField(person.Age);
                    csvWriter.WriteField(person.CountryName);
                    csvWriter.WriteField(person.Address);
                    csvWriter.WriteField(person.ReceiveNewsLetter);

                    csvWriter.NextRecord();
                    await csvWriter.FlushAsync();
                }
            }
            memoryStream.Position = 0;

            return memoryStream;
        }

        public virtual async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                excelWorksheet.Cells["A1"].Value = "Person Name";
                excelWorksheet.Cells["B1"].Value = "Email";
                excelWorksheet.Cells["C1"].Value = "Date of Birth";
                excelWorksheet.Cells["D1"].Value = "Age";
                excelWorksheet.Cells["E1"].Value = "Gender";
                excelWorksheet.Cells["F1"].Value = "Country Name";
                excelWorksheet.Cells["G1"].Value = "Address";
                excelWorksheet.Cells["H1"].Value = "ReceiveNewsLetter";

                using (ExcelRange headerCells = excelWorksheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }
                int row = 2;

                List<PersonResponse> persons = await GetAllPersons();

                foreach (var person in persons)
                {
                    excelWorksheet.Cells[row, 1].Value = person.PersonName;
                    excelWorksheet.Cells[row, 2].Value = person.Email;
                    excelWorksheet.Cells[row, 3].Value = person.DateOfBirth?.ToString("dd-MM-yyyy") ?? "";
                    excelWorksheet.Cells[row, 4].Value = person.Age;
                    excelWorksheet.Cells[row, 5].Value = person.Gender;
                    excelWorksheet.Cells[row, 6].Value = person.CountryName;
                    excelWorksheet.Cells[row, 7].Value = person.Address;
                    excelWorksheet.Cells[row, 8].Value = person.ReceiveNewsLetter;

                    row++;
                }

                excelWorksheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
