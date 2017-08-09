﻿using System.Linq;
using FizzWare.NBuilder.Tests.Integration.Models;
using FizzWare.NBuilder.Tests.Integration.Models.Repositories;
using Shouldly;
using Xunit;


namespace FizzWare.NBuilder.Tests.Integration
{
    
    public class PersistenceTests
    {
        private BuilderSettings builderSettings;

        public PersistenceTests()
        {
            //this.builderSettings = new PersistenceTestsBuilderSetup().SetUp();
            this.Repositories = new RepositoryBuilderSetup();
            this.builderSettings = this.Repositories.DoSetup();
        }

        public RepositoryBuilderSetup Repositories { get; set; }


        [Fact]
        public void PersistingASingleObject()
        {
            new Builder(builderSettings).CreateNew<Product>().Persist();

            // Go directly to the database to do some asserts
            var dataTable = this.Repositories.Products.GetAll();

            dataTable.Count.ShouldBe(1, "Count");

            dataTable[0].Title.ShouldBe("Title1");
            dataTable[0].Description.ShouldBe("Description1");
            dataTable[0].PriceBeforeTax.ShouldBe(1m, "PriceBeforeTax");
            dataTable[0].QuantityInStock.ShouldBe(1, "QuantityInStock");
        }

        [Fact]
        public void PersistingASingleTaxTypeAndAListOf100Products()
        {
            var builder = new Builder(builderSettings);
            var taxType = builder.CreateNew<TaxType>().Persist();

            builder.CreateListOfSize<Product>(100)
                .All()
                .With(x => x.TaxType = taxType)
                .Persist(); // NB: Persistence is setup in the RepositoryBuilderSetup class

            var dbProducts = this.Repositories.Products.GetAll();

            dbProducts.Count.ShouldBe(100);
        }


        [Fact]
        public void PersistingAListOfProductsAndCategories()
        {
            const int numProducts = 500;
            const int numCategories = 50;
            const int numCategoriesForEachProduct = 5;

            var builder = new Builder(builderSettings);
            var categories = builder
                .CreateListOfSize<Category>(numCategories)
                .Persist();

            builder
                .CreateListOfSize<Product>(numProducts)
                .All()
                .With(x => x.Categories = Pick<Category>.
                    UniqueRandomList(With.Exactly(numCategoriesForEachProduct).Elements)
                    .From(categories)
                    .ToList())
                .Persist(); // NB: Persistence is setup in the RepositoryBuilderSetup class

            var productsTable = this.Repositories.Products.GetAll();
            var categoriesTable = this.Repositories.Categories.GetAll();

            productsTable.Count.ShouldBe(numProducts, "products");
            categoriesTable.Count.ShouldBe(numCategories, "categories");
        }

        // TODO: Add CreatingAHierarchyOfCategoriesAndAddingProducts
    }
}