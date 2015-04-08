using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Sketch.DataAccessLayer;

namespace AutoMapper.Sketch
{
    class Program
    {
        static void Main(string[] args)
        {
            MyPrototype();

            UnworkedSimpleWithProjection();
        }

        private static void MyPrototype()
        {
            var repo = new ContactRepository();
            repo.TestConnection();

            var contactsQuery = repo.GetContactsUsingPrototype();


            #region Look at debug view: contactsQuery.Expression.DebugView
            /*
                My idea is replace original query with a mock one.
                Now user able to build his own expression tree 
                    and we will convert this tree to original source at execute time
             
             .Constant<System.Linq.EnumerableQuery`1[AutoMapper.Sketch.DataAccessLayer.Contact]>(AutoMapper.Sketch.DataAccessLayer.Contact[])
             
             
             */
            #endregion

            var allContactsAmount = contactsQuery.Count();

            // here we build query to empty collection of Contacts
            var query = repo.GetContactsUsingPrototype()
                .Where(c => c.Town == "Seattle")
                .OrderBy(c => c.DayOfBirth);


            // here when we start execute query AutoMapper replaces source to original and converts expression tree
            var contacts = query.ToList();


            Debug.Assert(contacts.Count > 0);
            Debug.Assert(allContactsAmount > contacts.Count);
        }

        private static void UnworkedSimpleWithProjection()
        {
            var repo = new ContactRepository();
            repo.TestConnection();

            var contactsQuery = repo.GetContactsUsingProjection();

            #region Look at debug view: contactsQuery.Expression.DebugView

            /* 
                AutoMapper has already modified Expression tree and added select statement.
                    In case of local collection it will force query to enumerate all items,
                to convert them into Contact and only after that query will apply next expressions on new collection
                    
                    In case of OData client it will lead to exception, because OData Client do not allow to add any expressions after 'Select'
             
             
            .Call System.Linq.Queryable.Select(
                .Extension<Microsoft.OData.Client.ResourceSetExpression>,
                '(.Lambda #Lambda1<System.Func`2[Northwind.NorthwindModel.Employee,AutoMapper.Sketch.DataAccessLayer.Contact]>))

            .Lambda #Lambda1<System.Func`2[Northwind.NorthwindModel.Employee,AutoMapper.Sketch.DataAccessLayer.Contact]>(Northwind.NorthwindModel.Employee $dto)
            {
                .New AutoMapper.Sketch.DataAccessLayer.Contact(){
                    BirthDate = .If (
                        ($dto.BirthDate).HasValue
                    ) {
                        (System.Nullable`1[System.DateTime])(($dto.BirthDate).Value).UtcDateTime
                    } .Else {
                        null
                    },
                    Country = $dto.Country,
                    Region = $dto.Region,
                    City = $dto.City,
                    Name = $dto.FirstName + " " + $dto.LastName,
                    JobPosition = $dto.Address
                }
            }
             */

            #endregion

            // code below will throw exception

            var allContactsAmount = contactsQuery.Count();

            var contacts = repo.GetContactsUsingProjection()
                .Where(c => c.Town == "Seattle")
                .OrderBy(c => c.DayOfBirth)
                .ToList();


            Debug.Assert(contacts.Count > 0);
            Debug.Assert(allContactsAmount > contacts.Count);
        }
    }
}
