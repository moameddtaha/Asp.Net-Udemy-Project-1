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
    public class PersonsSorterService : IPersonsSorterService
    {
        //Private field
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsSorterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsSorterService(IPersonsRepository personsRepository, ILogger<PersonsSorterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {

            _logger.LogInformation("GetSortedPersons of PersonsService");

            if (sortBy == null)
            {
                return Task.FromResult(allPersons);
            }

            var propertyInfo = typeof(PersonResponse).GetProperty(sortBy);

            if (propertyInfo == null)
            {
                return Task.FromResult(allPersons);
            }

            List<PersonResponse> sortedPersons = sortOrder == SortOrderOptions.ASC ? allPersons.OrderBy(person =>
            {
                var value = propertyInfo.GetValue(person);

                if (value == null)
                {
                    return string.Empty;
                }

                if (value is IComparable comparableValue)
                {
                    return comparableValue;
                }

                return value.ToString();
            }).ToList()
            : allPersons.OrderByDescending(person =>
            {
                var value = propertyInfo.GetValue(person);

                if (value == null)
                {
                    return string.Empty;
                }

                if (value is IComparable comparableValue)
                {
                    return comparableValue;
                }

                return value.ToString();
            }).ToList();
            return Task.FromResult(sortedPersons);
        }
    }
}
