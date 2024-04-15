
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Dashboard;

namespace Stimulsoft.Report.Gauge
{
    public class StiGaugeStyleXF26 : StiGaugeStyleXF
    {
        #region Properties.override
        public override bool AllowDashboard => true;

        public override string DashboardName => StiLocalization.Get("PropertyColor", "Turquoise");

        public override StiElementStyleIdent StyleIdent => StiElementStyleIdent.Turquoise;
        #endregion

        #region Methods.override
        public override StiGaugeStyleXF CreateNew() => new StiGaugeStyleXF26();
        #endregion

        public StiGaugeStyleXF26()
        {
            this.Core = new StiGaugeStyleCoreXF26();
        }
    }
}