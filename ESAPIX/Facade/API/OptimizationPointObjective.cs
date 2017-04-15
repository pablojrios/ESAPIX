using System;
using System.Collections.Generic;
using System.Linq;
using X = ESAPIX.Facade.XContext;

namespace ESAPIX.Facade.API
{
    public class OptimizationPointObjective : ESAPIX.Facade.API.OptimizationObjective
    {
        public OptimizationPointObjective() { }
        public OptimizationPointObjective(dynamic client) { _client = client; }
        public ESAPIX.Facade.Types.DoseValue Dose
        {
            get
            {
                var local = this;
                return new ESAPIX.Facade.Types.DoseValue(local._client.Dose);
            }
        }
        public ESAPIX.Facade.Types.OptimizationObjectiveOperator Operator
        {
            get
            {
                var local = this;
                return (ESAPIX.Facade.Types.OptimizationObjectiveOperator)local._client.Operator;
            }
        }
        public System.Double Priority
        {
            get
            {
                var local = this;
                return X.Instance.CurrentContext.GetValue<System.Double>((sc) => { return local._client.Priority; });
            }
        }
        public System.Double Volume
        {
            get
            {
                var local = this;
                return X.Instance.CurrentContext.GetValue<System.Double>((sc) => { return local._client.Volume; });
            }
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            var local = this;
            X.Instance.CurrentContext.Thread.Invoke(() =>
            {
                local._client.WriteXml(writer);
            });

        }
    }
}