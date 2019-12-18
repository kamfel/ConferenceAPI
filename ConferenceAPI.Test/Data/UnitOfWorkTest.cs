using AutoFixture;
using AutoFixture.AutoMoq;
using ConferenceAPI.Core.Models;
using ConferenceAPI.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;

namespace ConferenceAPI.Test.Data
{
    public class UnitOfWorkTest
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        { 
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<DbContextOptions>(() => new DbContextOptions<DbContext>());
            _fixture.Register(() => Mock.Of<DbContext>());
        }

        [Test]
        public void GetRepository_ValidEntity_Returns()
        {
            var mock = _fixture.Freeze<Mock<DbContext>>();
            mock.Setup(p => p.Set<Room>()).Returns(Mock.Of<DbSet<Room>>());
            var sut = _fixture.Create<UnitOfWork>();

            Assert.DoesNotThrow(() => sut.GetRepository<Room>());
        }

        [Test]
        public void GetRepository_InvalidEntity_Throws()
        {
            var mock = _fixture.Freeze<Mock<DbContext>>();
            mock.Setup(p => p.Set<string>()).Throws(new InvalidOperationException());
            var sut = _fixture.Create<UnitOfWork>();

            Assert.Throws<InvalidOperationException>(() => sut.GetRepository<string>());
        }
    }
}
