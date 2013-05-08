using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using DapperExtensions;
using System.Configuration;
using Dapper;
using Microsoft.Practices.ServiceLocation;
using Raven.Client;
using System.Linq.Expressions;

namespace RssAlerter.Mvc.Web.Models.Persistence
{
    public class RssAlerterDB : IDisposable
    {
        readonly IDocumentStore documentStore;

        public RssAlerterDB()
            : this(ServiceLocator.Current.GetInstance<IDocumentStore>())
        { }

        public RssAlerterDB(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public IEnumerable<T> GetList<T>()
        {
            using (var session = documentStore.OpenSession())
            {
                return GetList<T>(session);
            }
        }

        public IQueryable<T> GetList<T>(IDocumentSession session)
        {
            return session.Query<T>();
        }

        public IEnumerable<T> GetList<T>(IEnumerable<string> ids)
        {
            using (var session = documentStore.OpenSession())
            {
                return GetList<T>(session, ids);
            }
        }

        public IEnumerable<T> GetList<T>(IDocumentSession session, IEnumerable<string> ids)
        {
            return session.Load<T>(ids);
        }

        public IEnumerable<T> GetList<T>(Expression<Func<T, bool>> predicate)
        {
            using (var session = documentStore.OpenSession())
            {
                return GetList<T>(session, predicate).ToList();
            }
        }

        public IQueryable<T> GetList<T>(IDocumentSession session, Expression<Func<T, bool>> predicate)
        {
            return session.Query<T>().Where(predicate);
        }

        public T Get<T>(dynamic id)
        {
            using (var session = documentStore.OpenSession())
            {
                return Get<T>(id);
            }
        }

        public T Get<T>(IDocumentSession session, dynamic id)
        {
            return session.Load<T>(id);
        }

        public void Store<T>(T obj)
        {
            using (var session = documentStore.OpenSession())
            {
                Store<T>(session, obj);

                session.SaveChanges();
            }
        }

        public void Store<T>(IDocumentSession session, T obj)
        {
            session.Store(obj);
        }

        public void Delete<T>(T obj)
        {
            using (var session = documentStore.OpenSession())
            {
                Delete<T>(obj);

                session.SaveChanges();
            }
        }

        public void Delete<T>(IDocumentSession session, T obj)
        {
            session.Delete<T>(obj);
        }

        public IQueryable<T> Query<T>(IDocumentSession session)
        {
            return session.Query<T>();
        }

        public IDocumentSession BeginSession()
        {
            return documentStore.OpenSession();
        }

        public void Dispose()
        {
            //if (_cn != null)
            //{
            //    _cn.Close();
            //    _cn.Dispose();
            //}
        }
    }
}
