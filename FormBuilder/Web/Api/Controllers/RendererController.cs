using System;
using System.Web.Http;

using Newtonsoft.Json;

using CompositeC1Contrib.FormBuilder.Web.Api.Models;
using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Web.Api.Controllers
{
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
            if (type == null || !typeof(IFormFormRenderer).IsAssignableFrom(type))
            {
                return BadRequest();
            }

            var renderer = (IFormFormRenderer)Activator.CreateInstance(type);
            var summary = renderer.ValidationSummary(model.Errors);

            return Ok(summary);
        }

        [HttpGet]
        [ActionName("settings")]
        public IHttpActionResult GetSettings([FromUri]string type)
        {
            var rendererType = Type.GetType(type);
            if (rendererType == null || !typeof(IFormFormRenderer).IsAssignableFrom(rendererType))
            {
                return NotFound();
            }

            var renderer = Activator.CreateInstance(rendererType) as IFormFormRenderer;
            var json = JsonConvert.SerializeObject(renderer);

            return Ok(json);
        }
    }
}
