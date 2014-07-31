using System.Collections.Generic;
using System.Web.Http;

namespace CompositeC1Contrib.FormBuilder.Web.Api.Controllers
{
    [C1ConsoleAuthorize]
    public class FormSubmitsController : ApiController
    {
        public IEnumerable<FormSubmit> Get(string name)
        {
            return SaveFormSubmitFacade.LoadSubmits(name);
        }
    }
}
