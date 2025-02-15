using AutoFixture;
using Moq;
using ServiceContracts;
using CRUDExample.Controllers;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonsGetterService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        private readonly Mock<IPersonsGetterService> _personsServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly IFixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _personsServiceMock = new Mock<IPersonsGetterService>();
            _countriesServiceMock = new Mock<ICountriesService>();
            _loggerMock = new Mock<ILogger<PersonsController>>();

            _personsService = _personsServiceMock.Object;
            _countriesService = _countriesServiceMock.Object;
            _logger = _loggerMock.Object;
        }

        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            // Arrange
            List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger);

            _personsServiceMock
                .Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(persons_response_list);

            _personsServiceMock
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

            _countriesServiceMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            _personsServiceMock
                .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger);

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

            _personsServiceMock
                .Setup(temp => temp.GetPersonbyPersonID(It.IsAny<Guid?>()))
                .ReturnsAsync(null as PersonResponse);

            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger);

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

            _personsServiceMock
                .Setup(temp => temp.GetPersonbyPersonID(personResponse.PersonID))
                .ReturnsAsync(personResponse);

            _countriesServiceMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            var expectedSelectListItems = countries.Select(country => new SelectListItem
            {
                Text = country.CountryName,
                Value = country.CountryId.ToString()
            });

            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger);

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

            _personsServiceMock
                .Setup(temp => temp.GetPersonbyPersonID(It.IsAny<Guid?>()))
                .ReturnsAsync(null as PersonResponse);

            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger);

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

            _personsServiceMock
                .Setup(temp => temp.GetPersonbyPersonID(perosnUpdateRequest.PersonID))
                .ReturnsAsync(personResponse);

            _personsServiceMock
                .Setup(temp => temp.UpdatePerson(perosnUpdateRequest))
                .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger);

            // Act

            IActionResult result = await personsController.Edit(perosnUpdateRequest);

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectResult.ActionName);
        }


        #endregion
    }
}
