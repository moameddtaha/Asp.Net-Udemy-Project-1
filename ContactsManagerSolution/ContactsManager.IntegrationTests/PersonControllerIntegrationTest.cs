using FluentAssertions;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Entities;


namespace CRUDTests
{
    public class PersonControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {

        private readonly HttpClient _client;

        public PersonControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        #region Index

        [Fact]
        public async Task Index_ToReturnView()
        {
            // Arrange

            // Act

            HttpResponseMessage response = await _client.GetAsync("/Persons/Index");

            // Assert
            response.Should().BeSuccessful(); //2xx

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(responseBody);

            var document = html.DocumentNode;

            document.QuerySelectorAll("table.persons").Should().NotBeNull();

        }

        #endregion


        #region Create View

        [Fact]
        public async Task Create_Get_ShouldReturnCreateView()
        {
            // Arrange

            //Act
            HttpResponseMessage response = await _client.GetAsync("/Persons/Create");

            //Assert
            response.Should().BeSuccessful(); // 2xx

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(responseBody);

            var document = html.DocumentNode;

            document.QuerySelectorAll("form#create-person-form").Should().NotBeNull();

        }
        #endregion
    }
}
