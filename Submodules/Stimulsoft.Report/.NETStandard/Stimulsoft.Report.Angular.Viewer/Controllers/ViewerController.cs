using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report;
using Stimulsoft.Report.Web;
using Stimulsoft.Report.Angular;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;

namespace NetCoreAngularApplication.Controllers
{
    [Controller]
    public class ViewerController : Controller
    {
        /*[HttpGet]
        public IActionResult Get()
        {
            // Setting the required options on the server side
            /*var requestParams = StiAngularViewer.GetRequestParams(this);
			if (requestParams.Action == StiAction.Undefined)
			{
				var options = new StiAngularViewerOptions();

				return StiAngularViewer.GetScriptsResult(this, options);
			}*/

            /*return StiAngularViewer.ProcessRequestResult(this);
        }*/

        [HttpPost]
        public IActionResult InitViewer()
        {
            var requestParams = StiAngularViewer.GetRequestParams(this);
            var options = new StiAngularViewerOptions();
            options.Actions.ViewerEvent = "ViewerEvent";
            options.Toolbar.ShowDesignButton = true;
            options.Appearance.ScrollbarsMode = true;
            options.Appearance.ParametersPanelPosition = StiParametersPanelPosition.Left;
            //options.Localization = StiAngularHelper.MapPath(this, "Localization/de.xml");
            return StiAngularViewer.ViewerDataResult(requestParams, options);
        }

        [HttpPost]
        public IActionResult ViewerEvent()
        {
            var requestParams = StiAngularViewer.GetRequestParams(this);

            var reportName = "d_chart.mrt";
            //var reportName = "interactions/BookmarksAndHyperlinks.mrt";
            var httpContext = new Stimulsoft.System.Web.HttpContext(this.HttpContext);
            var optionsParams = httpContext.Request.Params["properties"]?.ToString();
            if (optionsParams != null)
            {
                var data = Convert.FromBase64String(optionsParams);
                var json = Encoding.UTF8.GetString(data);
                JContainer container = JsonConvert.DeserializeObject<JContainer>(json);
                foreach (JToken token in container.Children())
                {
                    if (((JProperty)token).Name == "reportName")
                    {
                        reportName = ((JProperty)token).Value.Value<string>();
                    }
                }
            }

            reportName = "interactions/BookmarksAndHyperlinks.mrt";

            if (requestParams.Action == StiAction.GetReport)
            {
                return GetReport(reportName);
            }

            return StiAngularViewer.ProcessRequestResult(this);
        }

        public IActionResult GetReport(string reportName)
        {
            var report = StiReport.CreateNewReport();
            var path = StiAngularHelper.MapPath(this, $"Reports/{reportName}");
            //var path = StiAngularHelper.MapPath(this, "Reports/ParametersInvoice.mrt");
            //var path = StiAngularHelper.MapPath(this, "Reports/MasterDetail.mrt");
            //var path = StiAngularHelper.MapPath(this, "Reports/Variables.mrt");
            //var path = StiAngularHelper.MapPath(this, "Reports/attach.mrt");
            report.Load(path);

            return StiAngularViewer.GetReportResult(this, report);
        }
    }
}