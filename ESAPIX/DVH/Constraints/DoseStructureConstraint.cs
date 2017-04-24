﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAPIX.Interfaces;
using ESAPIX.Extensions;
using ESAPIX.Facade.Types;
using ESAPIX.Facade.API;
using Newtonsoft.Json;

namespace ESAPIX.DVH.Constraints
{
    public abstract class DoseStructureConstraint : IPriorityConstraint
    {
        /// <summary>
        /// The string structure name corresponding to the structure ID in Eclipse. Separate with '&' character for multiple structures
        /// </summary>
        public string StructureName { get; set; }

        [JsonIgnore]
        public DoseValue ConstraintDose
        {
            get { return new DoseValue(this.Dose, Unit); }
            set { this.Dose = value.Dose; this.Unit = value.Unit; }
        }

        public virtual string Name { get { return ToString(); } }
        public string FullName { get { return $"{StructureName} {Name}"; } }

        /// <summary>
        /// The dose value component of the constraint dose - Used for text storage
        /// </summary>
        public double Dose { get; set; }

        /// <summary>
        /// The dose unit component of the constraint dose - Used for text storage
        /// </summary>
        public DoseValue.DoseUnit Unit { get; set; }

        public PriorityType Priority { get; set; }

        public abstract ConstraintResult Constrain(PlanningItem pi);

        public ConstraintResult CanConstrain(PlanningItem pi)
        {
            var message = string.Empty;
            //Check for null plan
            var valid = pi != null;
            if (!valid) { return new ConstraintResult(this, ResultType.NOT_APPLICABLE, "Plan is null"); }

            //Check structure exists
            var structures = StructureName.Split('&');
            foreach (var s in structures)
            {
                valid = pi.ContainsStructure(s);
                if (!valid) { return new ConstraintResult(this, ResultType.NOT_APPLICABLE_MISSING_STRUCTURE, $"{s} isn't contoured in {pi.Id}"); }
            }

            //Check dose is calculated
            valid = pi.Dose != null;
            if (!valid) { return new ConstraintResult(this, ResultType.NOT_APPLICABLE_MISSING_DOSE, $"There is no dose calculated for {pi.Id}"); }

            return new ConstraintResult(this, ResultType.PASSED, string.Empty);
        }

        /// <summary>
        /// Splits structure names separated by & character
        /// </summary>
        [JsonIgnore]
        public string[] StructureNames
        {
            get { return StructureName.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries); }
        }

        /// <summary>
        /// Merges cumulative DVHs into one DVH based on absolute volume
        /// </summary>
        /// <param name="pi">the planning item containing the dose to query</param>
        /// <returns>the DVH as an array of DVHPoint values</returns>
        public DVHPoint[] GetMergedDVH(PlanningItem pi)
        {
            IEnumerable<Structure> structures = GetStructures(pi);
            var dvhs = structures.Select(s => pi.GetDVHCumulativeData(s, ConstraintDose.GetPresentation(), VolumePresentation.AbsoluteCm3, 0.1));
            return dvhs.MergeDVHs();
        }

        /// <summary>
        /// Returns the structures in the planning item from the structure names property
        /// </summary>
        /// <param name="pi">the planning item to query</param>
        /// <returns>the collection of structures</returns>
        public IEnumerable<Structure> GetStructures(PlanningItem pi)
        {
            return StructureNames.Select(s => pi.GetStructure(s));
        }

        public ResultType GetFailedResultType()
        {
            switch (Priority)
            {
                case PriorityType.MAJOR_DEVIATION:
                case PriorityType.PRIORITY_1: return ResultType.ACTION_LEVEL_3;
                default: return ResultType.ACTION_LEVEL_1;
            }
        }
    }
}