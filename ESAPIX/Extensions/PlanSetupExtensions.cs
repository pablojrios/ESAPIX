﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAPIX.Interfaces;
using ESAPIX.Facade.API;
using ESAPIX.Facade.Types;

namespace ESAPIX.Extensions
{
    public static class PlanSetupExtensions
    {
        public static bool IsEcomp(this PlanSetup ps)
        {
            return ps.Beams.Any(b => b.MLCPlanType == MLCPlanType.DoseDynamic && b.CalculationLogs.Any(c => c.Category == "Compensator"));
        }
    }
}
