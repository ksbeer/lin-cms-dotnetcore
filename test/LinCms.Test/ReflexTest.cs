﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinCms.Aop.Filter;
using LinCms.Controllers.Cms;
using LinCms.Data;
using LinCms.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace LinCms.Test
{
    public class ReflexTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ReflexTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void GetAllActionByController()
        {
            Type t = typeof(LogController);
            MethodInfo[] controllerMethods = t.GetMethods();
            StringBuilder methodsNameAppend = new StringBuilder();

            foreach (MethodInfo t1 in controllerMethods)
            {
                methodsNameAppend.Append(t1.Name + ";");
                _testOutputHelper.WriteLine(t1.Name);
            }
        }

        [Fact]
        public void TestOutputHelpler()
        {
            _testOutputHelper.WriteLine("console test output helper");
        }


        //"LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","查询日志记录的用户","日志"
        //"LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","查询所有日志","日志"
        //"LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","搜索日志","日志"
        [Fact]
        public void LogControllerGetLinCmsAuthorize()
        {
            Type t = typeof(LogController);
            MethodInfo[] controllerMethods = t.GetMethods();

            foreach (MethodInfo t1 in controllerMethods)
            {
                IEnumerable<Attribute> v = t1.GetCustomAttributes();

                foreach (Attribute v1 in v)
                {
                    if (v1 is LinCmsAuthorizeAttribute vv)
                    {
                        _testOutputHelper.WriteLine(vv.ToString());
                    }
                }
            }
        }

        //Microsoft.AspNetCore.Mvc.RouteAttribute
        //Microsoft.AspNetCore.Mvc.ApiControllerAttribute
        //Microsoft.AspNetCore.Mvc.ControllerAttribute
        [Fact]
        public void GetLinCmsAttribute()
        {
            IEnumerable<Attribute> attributes = typeof(LogController).GetTypeInfo().GetCustomAttributes();

            foreach (Attribute attribute in attributes)
            {
                _testOutputHelper.WriteLine(attribute.ToString());
            }
        }


        /// <summary>
        /// 获取方法上的LinCmsAuthorizeAttribute
        /// </summary>
        //"LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","Permission:查询日志记录的用户","Module:日志","Roles:","Policy:","AuthenticationSchemes:"
        //"LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","Permission:查询所有日志","Module:日志","Roles:","Policy:","AuthenticationSchemes:"
        //"LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","Permission:搜索日志","Module:日志","Roles:","Policy:","AuthenticationSchemes:"
        //"LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","Permission:查看lin的信息","Module:信息","Roles:","Policy:","AuthenticationSchemes:"
        [Fact]
        public void GetAssemblyMethodsAttributes()
        {
            List<Type> assembly = typeof(Startup.Startup).Assembly.GetTypes().AsEnumerable()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();

            assembly.ForEach(r =>
            {
                foreach (MethodInfo methodInfo in r.GetMethods())
                {
                    foreach (Attribute attribute in methodInfo.GetCustomAttributes())
                    {
                        if (attribute is LinCmsAuthorizeAttribute linCmsAuthorize)
                        {
                            _testOutputHelper.WriteLine(linCmsAuthorize.ToString());
                        }
                    }
                }
            });
        }


        /// <summary>
        /// 获取控制器上的LinCmsAuthorizeAttribute
        /// </summary>
        /// "LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","Permission:","Module:","Roles:Administrator","Policy:","AuthenticationSchemes:"
        [Fact]
        public void GetControllerAttributes()
        {
            List<Type> assembly = typeof(Startup.Startup).Assembly.GetTypes().AsEnumerable()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();

            assembly.ForEach(d =>
            {
                LinCmsAuthorizeAttribute linCmsAuthorize = d.GetCustomAttribute<LinCmsAuthorizeAttribute>();
                if (linCmsAuthorize != null)
                {
                    _testOutputHelper.WriteLine(linCmsAuthorize.ToString());
                }
            });
        }

        //LinCms.Zero.Data.PermissionDto Permission:查询日志记录的用户、Module:日志、Router:cms.log+get_users
        //LinCms.Zero.Data.PermissionDto Permission:查询所有日志、Module:日志、Router:cms.log+get_logs
        //LinCms.Zero.Data.PermissionDto Permission:搜索日志、Module:日志、Router:cms.log+get_user_logs
        //LinCms.Zero.Data.PermissionDto Permission:查看lin的信息、Module:信息、Router:.test+info
        [Fact]
        public void ReflexHelperTest()
        {
            List<PermissionDefinition> attributes = ReflexHelper.GetAssemblyLinCmsAttributes();
            foreach (PermissionDefinition attribute in attributes)
            {
                _testOutputHelper.WriteLine(attribute.ToString());
            }
        }

        //LinCms.Zero.Domain.LinPermission Auth:查询日志记录的用户、Module:日志
        //LinCms.Zero.Domain.LinPermission Auth:查询所有日志、Module:日志
        //LinCms.Zero.Domain.LinPermission Auth:搜索日志、Module:日志
        //LinCms.Zero.Domain.LinPermission Auth:查看lin的信息、Module:信息
        [Fact]
        public void ConvertToTree()
        {
            List<PermissionDefinition> linCmsAttributes = ReflexHelper.GetAssemblyLinCmsAttributes();

            dynamic obj = ReflexHelper.AuthorizationConvertToTree(linCmsAttributes);

            string jsonSerializeObject = JsonConvert.SerializeObject(obj);

        }


        [Fact]
        public void Test()
        {
            List<Assembly> assem = AppDomain.CurrentDomain.GetAssemblies().Where(r => r.FullName.Contains("LinCms.")).ToList();
        }

        [Fact]
        public void RelfexGetCustomAttributes()
        {
            var entityTypes = ReflexHelper.GetTypesByTableAttribute();
            Assert.True(entityTypes.Length > 0);
        }

        [Fact]
        public void GetTypes()
        {
            var entityTypes = ReflexHelper.GetTypesByNameSpace();
            Assert.True(entityTypes.Length > 0);
        }
    }
}
