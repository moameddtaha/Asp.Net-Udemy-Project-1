using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using EntityFrameworkCoreMock;
using Moq;
using AutoFixture;
using OfficeOpenXml.FormulaParsing.Ranges;
using FluentAssertions;
using RepoCon;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesAdderService _countriesAdderService;
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ICountriesUploaderService _countriesUploaderService;

        private readonly IFixture _fixture;

        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;

        // constructor
        public CountriesServiceTest()
        {
            _fixture = new Fixture();

            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            _countriesAdderService = new CountriesAdderService(_countriesRepository);
            _countriesGetterService = new CountriesGetterService(_countriesRepository);
            _countriesUploaderService = new CountriesUploaderService(_countriesRepository);
        }

        #region AddCountry

        //When ContryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            // Arrange
            CountryAddRequest? request = null;

            //Act
            Func<Task> func = async () =>
            {
                await _countriesAdderService.AddCountry(request);
            };

            //Assert
            await func.Should().ThrowAsync<ArgumentNullException>();
        }

        //When the Country.Name is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
        {
            // Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, null as string)
                .Create();

            //Act
            Func<Task> func = async () =>
            {
                await _countriesAdderService.AddCountry(request);
            };

            //Assert
            await func.Should().ThrowAsync<ArgumentException>();
        }

        //When the Country.Name is duplicate , it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
        {
            // Arrange
            CountryAddRequest? first_country_request = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "USA")
                .Create();

            Country first_country = first_country_request.ToCountry();

            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(first_country);

            _countriesRepositoryMock
                .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);

            CountryResponse first_country_from_add_country = await _countriesAdderService.AddCountry(first_country_request);

            //Act
            Func<Task> func = async () =>
            {
                _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(first_country);

                _countriesRepositoryMock
                    .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                    .ReturnsAsync(first_country);

                await _countriesAdderService.AddCountry(first_country_request);
            };

            //Assert
            await func.Should().ThrowAsync<ArgumentException>();
        }

        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_FullCountryDetails_ToBeSuccessful()
        {
            // Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .Create();

            Country country = request.ToCountry();
            CountryResponse country_response = country.ToCountryResponse();

            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            _countriesRepositoryMock
                 .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                 .ReturnsAsync(null as Country);

            //Act
            CountryResponse response = await _countriesAdderService.AddCountry(request);

            country.CountryId = response.CountryId;
            country_response.CountryId = response.CountryId;

            //Assert
            response.CountryId.Should().NotBe(Guid.Empty);
            Guid.TryParse(response.CountryId.ToString(), out _).Should().BeTrue();
            response.Should().BeEquivalentTo(country_response);
        }

        #endregion

        #region GetAllCountries

        // The list of countries should be empty by default (before adding any countries)
        [Fact]
        public async Task GetAllCountries_ToBeEmptyList()
        {
            //Arrange
            _countriesRepositoryMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(new List<Country>());

            //Act
            List<CountryResponse> actual_country_response_list = await _countriesGetterService.GetAllCountries();

            //Assert
            actual_country_response_list.Should().BeEmpty();

        }

        // The list of countries should be empty by default (before adding any countries)
        [Fact]
        public async Task GetAllCountries_ShouldHaveFewCountries()
        {
            //Arange

            List<Country> countryList = new List<Country>()
            {
                _fixture.Build<Country>()
                .With(temp => temp.Persons, null as List<Person>)
                .Create(),

                _fixture.Build<Country>()
                .With(temp => temp.Persons, null as List<Person>)
                .Create()
            };

            List<CountryResponse> expected_country_response_list = countryList.Select(temp => temp.ToCountryResponse()).ToList();

            _countriesRepositoryMock
               .Setup(temp => temp.GetAllCountries())
               .ReturnsAsync(countryList);

            //Act
            List<CountryResponse> actual_country_response_list = await _countriesGetterService.GetAllCountries();

            actual_country_response_list.Should().BeEquivalentTo(expected_country_response_list);
        }

        #endregion

        #region GetCountryByCountryID

        // If we supply null as CountryID is should return null as  CountryResponse
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
        {
            //Arrange
            Guid? countryID = null;

            //Act
            CountryResponse? country_reponse_from_get_method = await _countriesGetterService.GetCountryByCountryID(countryID);

            //Assert
            country_reponse_from_get_method.Should().BeNull();
        }

        //If we supply a valid country id, it should return the matching country details as CountryResponse object
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessful()
        {
            //Arrange
            Country country = _fixture.Build<Country>()
                .With(temp => temp.Persons, null as List <Person>)
                .Create();

            CountryResponse country_response = country.ToCountryResponse();

            _countriesRepositoryMock
                .Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
                .ReturnsAsync(country);

            //Act
            CountryResponse? country_reponse_from_get = await _countriesGetterService.GetCountryByCountryID(country.CountryId);

            //Assert
            country_reponse_from_get.Should().BeEquivalentTo(country_response);
        }

        #endregion

    }
}
