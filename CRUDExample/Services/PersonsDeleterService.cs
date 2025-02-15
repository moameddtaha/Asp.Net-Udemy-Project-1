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
    public class PersonsDeleterService : IPersonsDeleterService
    {
        //Private field
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsDeleterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsDeleterService(IPersonsRepository personsRepository, ILogger<PersonsDeleterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }

            Person? person = await _personsRepository.GetPersonbyPersonID(personID.Value);

            if (person == null)
            {
                return false;
            }

            await _personsRepository.DeletePersonByPersonID(personID.Value);

            return true;
        }
    }
}
