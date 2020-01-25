using AutoFixture;
using AutoMapper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceAPI.Test
{
    public class MappingTest
    {
        private IMapper _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new ConferenceProfile())));

        [Test]
        public void VerifyMappings()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new ConferenceProfile()));

            config.AssertConfigurationIsValid();
        }
    }
}
