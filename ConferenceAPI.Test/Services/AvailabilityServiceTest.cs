using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using ConferenceAPI.Core;
using Moq;
using NUnit.Framework;

namespace ConferenceAPI.Test.Services
{
    public class AvailabilityServiceTest
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }
    }
}
