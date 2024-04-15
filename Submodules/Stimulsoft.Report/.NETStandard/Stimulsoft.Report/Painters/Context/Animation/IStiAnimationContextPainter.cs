using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stimulsoft.Report.Painters.Context.Animation
{
    public interface IStiAnimationContextPainter<T>
    {
        List<T> Geoms { get; }

        StiAnimationEngine AnimationEngine { get; set; }
    }
}
