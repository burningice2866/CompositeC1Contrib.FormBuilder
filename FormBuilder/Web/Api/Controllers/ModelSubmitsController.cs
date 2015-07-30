using System.Collections.Generic;
using System.Web.Http;

namespace CompositeC1Contrib.FormBuilder.Web.Api.Controllers
{
    [C1ConsoleAuthorize]
    public class ModelSubmitsController : ApiController
    {
        public IEnumerable<ModelSubmit> Get(string name)
        {
            return SaveSubmitFacade.LoadSubmits(name);
        }
    }
}
