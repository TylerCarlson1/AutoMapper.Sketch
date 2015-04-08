using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.OData.Client;
using Northwind.NorthwindModel;
using Northwind.ODataWebExperimental.Northwind.Model;

namespace AutoMapper.Sketch.DataAccessLayer
{
    /// <summary>
    /// Only this repository should know about real data source and data models
    /// </summary>
    public class ContactRepository
    {
        private readonly string _serviceUri = "http://services.odata.org/V4/Northwind/Northwind.svc";
        private NorthwindEntities _odataClient;

        public ContactRepository()
        {
            InitOdataClient();
            CreateMap();
        }

        public IQueryable<Contact> GetContactsUsingProjection()
        {
            // this not work with odata because you modify query and add 'Select' expression
            return _odataClient.Employees.Project().To<Contact>();
        }

        public IQueryable<Contact> GetContactsUsingPrototype()
        {
            return _odataClient.Employees.UseAsDataSource().For<Contact>();
        }

        public void TestConnection()
        {
            var count = _odataClient.Employees.Count();
        }

        private void OdataClient_BuildingRequest(object sender, BuildingRequestEventArgs e)
        {
            // here you can see actual request 
            var uri = e.RequestUri;
            Console.WriteLine("path: {0}", uri.AbsolutePath);
            Console.WriteLine("params: {0}", uri.Query);
            Debugger.Break();
        }

        private static void CreateMap()
        {
            Mapper.CreateMap<Employee, Contact>()
                .ForMember(c => c.JobPosition, opt => opt.MapFrom(e => e.Title))
                .ForMember(c => c.DayOfBirth, opt => opt.MapFrom(e => e.BirthDate))
                .ForMember(c => c.Town, opt => opt.MapFrom(e => e.City))
                .ReverseMap()
                .ForMember(e => e.Title, opt => opt.MapFrom(c => c.JobPosition))
                .ForMember(e => e.City, opt => opt.MapFrom(c => c.Town))
                .ForMember(e => e.BirthDate, opt => opt.MapFrom(c => c.DayOfBirth));
        }

        private void InitOdataClient()
        {
            _odataClient = new NorthwindEntities(new Uri(_serviceUri));
            _odataClient.Credentials = CredentialCache.DefaultCredentials;
            _odataClient.BuildingRequest += OdataClient_BuildingRequest;
        }

    }
}
