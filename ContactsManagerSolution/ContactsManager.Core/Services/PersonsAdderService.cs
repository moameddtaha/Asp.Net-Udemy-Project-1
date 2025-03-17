using CsvHelper;
using CsvHelper.Configuration;
using Entities;
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
    public class PersonsAdderService : IPersonsAdderService
    {
        //Private field
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsAdderService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsAdderService(IPersonsRepository personsRepository, ILogger<PersonsAdderService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            //Model validations
            ValidationHelper.ModelValidation(request);

            //Convert request into Person type
            Person person = request.ToPerson();

            //Generate PersonID
            person.PersonID = Guid.NewGuid();

            //Add person object to persons list

            await _personsRepository.AddPerson(person);

            // TODO: Implement stored procedure for inserting person
            //await _db.sp_InsertPersons(person);

            //Convert person object into PersonResponse type
            return person.ToPersonResponse();

        }

    }
}
