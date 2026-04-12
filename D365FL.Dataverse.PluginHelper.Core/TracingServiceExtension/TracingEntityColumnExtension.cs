using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;


namespace D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension
{
    public static class TracingEntityColumnExtension
    {
        // TODO ensure tests for the below values in the switch as well as
        // -> OptionSetValueCollection (multi-select option sets)
        // -> EntityCollection(activity parties)
        // -> bool (two-option fields)
        // -> DateTime
        // -> decimal, double, int, long
        public static string GetTraceableValue(this KeyValuePair<string, object> attr)
        {
            string attributeValue = null;
            switch (attr.Value)
            {
                case EntityReference er:
                    var name = !string.IsNullOrEmpty(er.Name) ? er.Name : "NOT SET";
                    attributeValue = $"EntityReference(LogicalName={er.LogicalName}, Id={er.Id}, Name={name})";
                    // TODO handle Key Value Pair Ids
                    break;
                case OptionSetValue osv:
                    attributeValue = $"OptionSetValue({osv.Value})";
                    break;
                case Money money:
                    attributeValue = $"Money({money.Value})";
                    break;
                case AliasedValue av:
                    attributeValue = $"AliasedValue({av.EntityLogicalName}.{av.AttributeLogicalName} = {av.Value})";
                    break;
                case null:
                    attributeValue = "null";
                    break;
                default:
                    attributeValue = attr.Value.ToString();
                    break;
            }

            return attributeValue;
        }
    }
}
