using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DocumentTracking.API.Helpers
{
    //it will contain an extension method so we make it static
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeDatas<TSource>(
            this IEnumerable<TSource> source, string fields)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            //create a listto hold our ExpandoObjects
            var expandoObjectList = new List<ExpandoObject>();

            //creat a listwith PropertyInfo objects on TSource. Reflection is 
            //expensive, so rather than doing it for each object in the list, do 
            //it once and reuse the results. After all part of the reflection is on the 
            //type of the object (TSource), not on the instance
            var propertyInfoList = new List<PropertyInfo>();

            if(string.IsNullOrWhiteSpace(fields))
            {
                //all oublic properties should be in the ExpandoObject
                var propertyInfos = typeof(TSource)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance);

                propertyInfoList.AddRange(propertyInfos);
            } else
            {
                // the fields are separated by ",", so we split it
                var fieldsAfterSplit = fields.Split(',');

                foreach (var field in fieldsAfterSplit)
                {
                    //trim each field, as it might contain leading
                    //or trailing spaces, Can;t trim the var in foreach,
                    //so use another var
                    var propertyName = field.Trim();

                    //use reflection to get the property on the source object
                    //we need to include public ans instance, cos specifying a 
                    //flag overwrites the elready-existing binding flags
                    var propertyInfo = typeof(TSource)
                        .GetProperty(propertyName, BindingFlags.IgnoreCase |
                        BindingFlags.Public | BindingFlags.Instance);

                    if(propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasn't found on " +
                            $" {typeof(TSource)}");
                    }

                    //add propertyInfo to the list
                    propertyInfoList.Add(propertyInfo);
                }
            }

            // run through the source objects
            foreach (TSource sourceObject in source)
            {
                //create an ExpandoObject that will hold 
                //the selected properties and values
                var dataShapeObject = new ExpandoObject();

                //Get the value of each property we have to return.
                //For that, we run through the list
                foreach (var propertyInfo in propertyInfoList)
                {
                    //call GetValue on the the propertyinfo of the source
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    //add the field to the ExpandoObject
                    ((IDictionary<string, object>)dataShapeObject)
                        .Add(propertyInfo.Name, propertyValue);
                }

                expandoObjectList.Add(dataShapeObject);
            }

            //return the list
            return expandoObjectList;
        }
    
    }
}
