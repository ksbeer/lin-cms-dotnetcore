﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IGeekFan.Localization.FreeSql.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sample.Localization.Demo;

namespace Sample.Localization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITest test;

        public TestController(ITest test)
        {
            this.test = test;
        }

        [HttpGet]
        public string Get()
        {
            ICollection<LocalResource> resources = new List<LocalResource>() { new LocalResource("Hello", "Hello") };

            new List<LocalResource>().AddRange(resources);

            IEnumerable<LocalResource> resources2 = new List<LocalResource>() { new LocalResource("Hello", "Hello") };
            test.Insert(resources.ToList());
            test.Insert(resources.ToList());
            test.Insert(resources);
            test.Insert(resources2);

            test.Insert(new LocalResource("Hello", "Hello"));
            return "ok";
        }
    }
}
