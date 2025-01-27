
using Entities;
using EntityFrameworkCoreMock;
using Moq;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;
using Xunit.Sdk;
using AutoFixture;
using FluentAssertions;
using RepoCon;
using System.Linq.Expressions;
using Serilog;
using Microsoft.Extensions.Logging;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        // private fields
        private readonly IPersonsService _personService;

        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _personsRepository;

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        // constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;

            var diagnosticCOntextMock = new Mock<IDiagnosticContext>();
            var loggerMock = new Mock<ILogger<PersonsService>>();

            _personService = new PersonsService(_personsRepository, loggerMock.Object, diagnosticCOntextMock.Object);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException 
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();

        }


        //When we supply null value as PersonName, it should throw ArgumentException 
        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();

            Person person = personAddRequest.ToPerson();


            // When PersonRepository.AddPerson is called, it has to return the same "person" object
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When we supply a proper person details, it should insert the person into a persons list; and it should return an object of PersonResponse, whic includes with the newly generated person id
        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@example.com")
                .Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();

            // If we supply any argument value to the AddPerson method, it should return the same return value
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);
            person_response_expected.PersonID = person_response_from_add.PersonID;


            //Assert

            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
            person_response_from_add.Should().Be(person_response_expected);

        }

        #endregion

        #region GetPersonbyPersonID

        //If PersonID is null, return null as the PersonResponse.
        [Fact]
        public async Task GetPersonbyPersonID_NullPersonID_ToBeNull()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? person_pesponse_from_get = await _personService.GetPersonbyPersonID(personID);

            //Assert

            //Assert.Null(person_pesponse_from_get);

            person_pesponse_from_get.Should().BeNull();

        }

        // If PersonID is valid, return the corresponding PersonResponse with full details.
        [Fact]
        public async Task GetPersonbyPersonID_WithPersonID_ToBeSucessful()
        {
            //Arrange

            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "email@sample.com")
                .With(temp => temp.Country, null as Country)
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.GetPersonbyPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse? person_pesponse_from_get = await _personService.GetPersonbyPersonID(person.PersonID);

            //Assert
            //Assert.Equal(person_response, person_pesponse_from_get);

            person_pesponse_from_get.Should().Be(person_response_expected);
        }

        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_ToBeEmptyList()
        {
            // Arrange

            var person = new List<Person>();

            _personRepositoryMock.Setup(temp=> temp.GetAllPersons())
                .ReturnsAsync(person);

            //Act
            List<PersonResponse> person_from_get = await _personService.GetAllPersons();

            //Assert
            
            //Assert.Empty(person_from_get);

            person_from_get.Should().BeEmpty();
        }

        // Add a few persons, then ensure GetAllPersons() returns the same persons that were added.
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };

            List<PersonResponse> person_respons_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //Print person_respons_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (var person in person_respons_list_expected)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            //Act

            List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

            //Print person_respons_list_from_add
            _testOutputHelper.WriteLine("Actual: ");
            foreach (var person in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            //Assert

            persons_list_from_get.Should().BeEquivalentTo(person_respons_list_expected);
        }

        #endregion

        #region GetFilteredPersons

        // If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSeccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };

            List<PersonResponse> person_respons_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //Print person_respons_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (var person in person_respons_list_expected)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            //Act

            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            //Print person_respons_list_from_add
            _testOutputHelper.WriteLine("Actual: ");
            foreach (var person in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            //Assert
            persons_list_from_search.Should().BeEquivalentTo(person_respons_list_expected);

        }

        // If we add few persons; and then we will search based on person name with some  search string. it should return the matching persons.
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };

            List<PersonResponse> person_respons_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //Print person_respons_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (var person in person_respons_list_expected)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            //Act

            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "sa");

            //Print person_respons_list_from_add
            _testOutputHelper.WriteLine("Actual: ");
            foreach (var person in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            //Assert
            persons_list_from_search.Should().BeEquivalentTo(person_respons_list_expected);
        }

        #endregion

        #region GetSortedPersons

        //When we sort based on the PersonName in DESC, it should return persons list in descending orders on PersonName
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };

            List<PersonResponse> person_respons_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personRepositoryMock
                .Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            //Print person_respons_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (var person in person_respons_list_expected)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            List<PersonResponse> allPerson = await _personService.GetAllPersons();

            //Act
            List<PersonResponse> persons_list_from_sort = await _personService.GetSortedPersons(allPerson, nameof(Person.PersonName), SortOrderOptions.DESC);

            //Print person_respons_list_from_add
            _testOutputHelper.WriteLine("Actual: ");
            foreach (var person in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            //Assert
            persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
        }

        #endregion

        #region UpdateRequest

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    //Act
            //    await _personService.UpdatePerson(personUpdateRequest);
            //});

            Func<Task> func = async () =>
            {
                await _personService.UpdatePerson(personUpdateRequest);
            };

            await func.Should().ThrowAsync<ArgumentNullException>();
        }

        //When we supply invalid person id, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            // Arrange
            PersonUpdateRequest person_request_1 = _fixture.Build<PersonUpdateRequest>()
                .With(temp => temp.Email, "someone_1@example.com")
                .Create();

            //Assert

            Func<Task> func = async () =>
            {
                await _personService.UpdatePerson(person_request_1);
            };

            await func.Should().ThrowAsync<ArgumentException>();
        }

        //When the PersonName is null, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            // Arrange

            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            PersonUpdateRequest? personUpdateRequest = person_response_from_add.ToPersonUpdateRequest();

            //Assert

            Func<Task> func = async () =>
            {
                await _personService.UpdatePerson(personUpdateRequest);
            };

            await func.Should().ThrowAsync<ArgumentException>();

        }

        //First, add a new person and then try to update the person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.PersonName, "Rahman")
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest? person_update_request = person_response_expected.ToPersonUpdateRequest();
            
            _personRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            _personRepositoryMock.Setup(temp => temp.GetPersonbyPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

            //Assert
            //Assert.Equal(person_response_from_get, person_response_from_update);

            person_response_from_update.Should().Be(person_response_expected);
        }


        #endregion

        #region DeletePerson

        [Fact]
        //If an invalid PersonID been supplied it returns false
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            //Assert

            //Assert.False(isDeleted);

            isDeleted.Should().BeFalse();
        }

        [Fact]
        //If a valid PersonID been supplied it returns true
        public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
        {
            //Arrange

            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Female")
                .With(temp => temp.PersonName, "Rahman")
                .Create();

            _personRepositoryMock
                .Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _personRepositoryMock
                .Setup(temp => temp.GetPersonbyPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            bool is_deleted = await _personService.DeletePerson(person.PersonID);

            //Assert

            is_deleted.Should().BeTrue();
        }

        #endregion

    }
}
