﻿using ESAPIX.Extensions;
using VMS.TPS.Common.Model.API;

namespace ESAPIX.DVH.Constraints
{
    /// <summary>
    /// Checks to make sure the structure name is present in the planning item structure set, and structure is not empty
    /// </summary>
    public class StructureNameConstraint : IConstraint
    {
        public string Regex { get; set; }
        public string StructureName { get; set; }
        public string Name { get { return $"{StructureName} required"; } }
        public string FullName { get { return Name; } }

        public ConstraintResult CanConstrain(PlanningItem pi)
        {
            var message = string.Empty;
            //Check for null plan
            var valid = pi != null;
            if (!valid) { message = "Plan is null"; }

            //Check structure exists
            valid = valid && pi.GetStructureSet() != null;
            if (!valid) { message = $"No structure set in {pi.Id}"; }
            return new ConstraintResult(this, valid, message);
        }

        public ConstraintResult Constrain(PlanningItem pi)
        {
            string msg = string.Empty;
            var structure = pi.GetStructure(StructureName, Regex);
            bool? passed = false;
            if (structure != null)
            {
                passed = true;
                msg = $"{pi.Id} contains structure {StructureName}";

                if (structure.Volume < 0.0001)
                {
                    passed = false;
                    msg = $"{StructureName} is empty";
                }
            }
            return new ConstraintResult(this, passed, msg);
        }

        public override string ToString()
        {
            return $"Required Structure {StructureName}";
        }
    }
}