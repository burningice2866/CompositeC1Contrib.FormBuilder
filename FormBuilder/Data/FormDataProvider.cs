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
            var formId = dataId as ModelReferenceId;
            if (formId == null)
            {
                throw new InvalidOperationException();
            }

            var model = ModelsFacade.GetModel(formId.Name);
            if (model == null)
            {
                return null;
            }

            var dataSource = _context.CreateDataSourceId(dataId, typeof(IModelReference));

            return new ModelReference(model, dataSource) as T;
        }

        public IQueryable<T> GetData<T>() where T : class, IData
        {
            var models = ModelsFacade.GetModels().Select(c =>
            {
                var dataId = new ModelReferenceId
                {
                    Source = c.Source.Name,
                    Type = c.Type,
                    Name = c.Model.Name
                };

                var dataSourceId = _context.CreateDataSourceId(dataId, typeof(IModelReference));

                return new ModelReference(c.Model, dataSourceId);
            });


            return (IQueryable<T>)models.AsQueryable();
        }

        public IEnumerable<Type> GetSupportedInterfaces()
        {
            return new[] { typeof(IModelReference) };
        }
    }
}