using AutoFixture;
using Moq;
using ServiceContracts;
using CRUDExample.Controllers;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsSorterService _personsSorterService;

        private readonly ICountriesAdderService _countriesAdderService;
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ICountriesUploaderService _countriesUploaderService;

        private readonly ILogger<PersonsController> _logger;

        private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;
        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;

        private readonly Mock<ICountriesAdderService> _countriesAdderServiceMock;
        private readonly Mock<ICountriesGetterService> _countriesGetterServiceMock;
        private readonly Mock<ICountriesUploaderService> _countriesUploaderServiceMock;

        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly IFixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _personsGetterServiceMock = new Mock<IPersonsGetterService>();
            _personsAdderServiceMock = new Mock<IPersonsAdderService>();
            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();
            _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
            _personsSorterServiceMock = new Mock<IPersonsSorterService>();

            _countriesAdderServiceMock = new Mock<ICountriesAdderService>();
            _countriesGetterServiceMock = new Mock<ICountriesGetterService>();
            _countriesUploaderServiceMock = new Mock<ICountriesUploaderService>();

            _loggerMock = new Mock<ILogger<PersonsController>>();

            _personsGetterService = _personsGetterServiceMock.Object;
            _personsAdderService = _personsAdderServiceMock.Object;
            _personsUpdaterService = _personsUpdaterServiceMock.Object;
            _personsDeleterService = _personsDeleterServiceMock.Object;
            _personsSorterService = _personsSorterServiceMock.Object;

            _countriesAdderService = _countriesAdderServiceMock.Object;
            _countriesGetterService = _countriesGetterServiceMock.Object;
            _countriesUploaderService = _countriesUploaderServiceMock.Object;

            _logger = _loggerMock.Object;
        }

        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            // Arrange
            List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(_personsGetterService, _personsAdderService, _personsDeleterService, _personsUpdaterService, _personsSorterService, _countriesAdderService, _countriesGetterService, _countriesUploaderService, _logger);

            _personsGetterServiceMock
                .Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(persons_response_list);

            _personsSorterServiceMock
                .Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
                .ReturnsAsync(persons_response_list);

            // Act
            IActionResult result = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();

            viewResult.ViewData.Model.Should().Be(persons_response_list);
        }

        #endregion

        #region Create

        [Fact]
        public async Task Create_IfNoModelErrors_ToReturnRedirectToIndex()
        {
            // Arrange
            PersonAddRequest persons_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesGetterServiceMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            _personsAdderServiceMock
                .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personsGetterService, _personsAdderService, _personsDeleterService, _personsUpdaterService, _personsSorterService, _countriesAdderService, _countriesGetterService, _countriesUploaderService, _logger);

            // Act

            IActionResult result = await personsController.Create(persons_add_request);

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("index");
        }

        #endregion

        #region Edit

        [Fact]
        public async Task Edit_Get_PersonDoesNotExists_ShouldRedirectToIndex()
        {
            // Arrange

            _personsGetterServiceMock
                .Setup(temp => temp.GetPersonbyPersonID(It.IsAny<Guid?>()))
                .ReturnsAsync(null as PersonResponse);

            PersonsController personsController = new PersonsController(_personsGetterService, _personsAdderService, _personsDeleterService, _personsUpdaterService, _personsSorterService, _countriesAdderService, _countriesGetterService, _countriesUploaderService, _logger);

            // Act

            IActionResult actionResult = await personsController.Edit(Guid.NewGuid());

            // Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal("index", redirectToActionResult.ActionName);
            Assert.Equal("Persons", redirectToActionResult.ControllerName);
        }

        [Fact]
        public async Task Edit_Get_PersonExists_ShouldReturnEditViewWithPerson()
        {
            // Arrange

            var personResponse = _fixture.Build<PersonResponse>()
                .With(temp => temp.Gender, GenderOptions.Male.ToString())
                .Create();

            var countries = _fixture.Create<List<CountryResponse>>();

            _personsGetterServiceMock
                .Setup(temp => temp.GetPersonbyPersonID(personResponse.PersonID))
                .ReturnsAsync(personResponse);

            _countriesGetterServiceMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            var expectedSelectListItems = countries.Select(country => new SelectListItem
            {
                Text = country.CountryName,
                Value = country.CountryId.ToString()
            });

            PersonsController personsController = new PersonsController(_personsGetterService, _personsAdderService, _personsDeleterService, _personsUpdaterService, _personsSorterService, _countriesAdderService, _countriesGetterService, _countriesUploaderService, _logger);

            // Act

            IActionResult actionResult = await personsController.Edit(personResponse.PersonID);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(actionResult);

            PersonUpdateRequest model = Assert.IsType<PersonUpdateRequest>(viewResult.ViewData.Model);

            Assert.Equal(personResponse.PersonID, model.PersonID);

            viewResult.ViewData["Countries"].Should().BeEquivalentTo(expectedSelectListItems, options => options
            .WithStrictOrdering()
            .ComparingByMembers<SelectListItem>());
        }

        [Fact]
        public async Task Edit_Post_PersonDoesNotExists_ShouldRedirectToIndex()
        {
            // Arrange

            _personsGetterServiceMock
                .Setup(temp => temp.GetPersonbyPersonID(It.IsAny<Guid?>()))
                .ReturnsAsync(null as PersonResponse);

            PersonsController personsController = new PersonsController(_personsGetterService, _personsAdderService, _personsDeleterService, _personsUpdaterService, _personsSorterService, _countriesAdderService, _countriesGetterService, _countriesUploaderService, _logger);

            // Act

            IActionResult actionResult = await personsController.Edit(Guid.NewGuid());

            // Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal("index", redirectToActionResult.ActionName);
            Assert.Equal("Persons", redirectToActionResult.ControllerName);
        }


        [Fact]
        public async Task Edit_Post_ValidModelState_ShouldRedirectToIndex()
        {
            // Arrange

            var perosnUpdateRequest = _fixture.Build<PersonUpdateRequest>()
                .With(temp => temp.Gender, GenderOptions.Male)
                .With(temp => temp.Email, "Mock@gmail.com")
                .Create();

            var personResponse = _fixture.Build<PersonResponse>()
                .With(temp => temp.Gender, GenderOptions.Male.ToString())
                .With(temp => temp.Email, "Mock@gmail.com")
                .Create();

            _personsGetterServiceMock
                .Setup(temp => temp.GetPersonbyPersonID(perosnUpdateRequest.PersonID))
                .ReturnsAsync(personResponse);

            _personsUpdaterServiceMock
                .Setup(temp => temp.UpdatePerson(perosnUpdateRequest))
                .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(_personsGetterService, _personsAdderService, _personsDeleterService, _personsUpdaterService, _personsSorterService, _countriesAdderService, _countriesGetterService, _countriesUploaderService, _logger);

            // Act

            IActionResult result = await personsController.Edit(perosnUpdateRequest);

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectResult.ActionName);
        }

        #endregion
    }
}
