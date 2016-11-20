using System;
using System.Web.Http;

using CompositeC1Contrib.FormBuilder.Web.Api.Models;
using CompositeC1Contrib.FormBuilder.Web.UI.Rendering;

namespace CompositeC1Contrib.FormBuilder.Web.Api.Controllers
{
    [SetCultureFilter]
    public class RendererController : ApiController
    {
        [HttpPost]
        [ActionName("validationsummary")]
        public IHttpActionResult RenderValidationSummary(ValidationSummaryModel model)
        {
            if (String.IsNullOrEmpty(model.Renderer))
            {
                return BadRequest();
            }

            var type = Type.GetType(model.Renderer);
            if (type == null || !typeof(FormRenderer).IsAssignableFrom(type))
            {
                return BadRequest();
            }

            var renderer = (FormRenderer)Activator.CreateInstance(type);
            var summary = renderer.ValidationSummary(model.Errors);

            return Ok(summary);
        }
    }
}
