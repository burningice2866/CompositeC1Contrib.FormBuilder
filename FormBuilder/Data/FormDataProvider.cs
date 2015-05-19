using System;
using System.Collections.Generic;
using System.Linq;

using Composite.Data;
using Composite.Data.Plugins.DataProvider;

using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.Data
{
    public class FormDataProvider : IDataProvider
    {
        private DataProviderContext _context;
        public DataProviderContext Context
        {
            set { _context = value; }
        }

        public T GetData<T>(IDataId dataId) where T : class, IData
        {
            var formId = dataId as FormDataId;
            if (formId == null)
            {
                throw new InvalidOperationException();
            }

            var form = FormModelsFacade.GetModel(formId.Name);
            if (form == null)
            {
                return null;
            }

            var dataSource = _context.CreateDataSourceId(dataId, typeof(IForm));

            return new FormData(form, dataSource) as T;
        }

        public IQueryable<T> GetData<T>() where T : class, IData
        {
            var models = FormModelsFacade.GetModels().Select(c =>
            {
                var dataId = new FormDataId
                {
                    Source = c.Source.Name,
                    Type = c.Type,
                    Name = c.Model.Name
                };

                var dataSourceId = _context.CreateDataSourceId(dataId, typeof(IForm));

                return new FormData(c.Model, dataSourceId);
            });


            return (IQueryable<T>)models.AsQueryable();
        }

        public IEnumerable<Type> GetSupportedInterfaces()
        {
            return new[] { typeof(IForm) };
        }
    }
}