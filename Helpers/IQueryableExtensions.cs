using DocumentTracking.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace DocumentTracking.API.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, 
            Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if(mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }

            if(string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            var orderByString = string.Empty;

            //the orderBy string is separated by ",", so we need to split it
            var orderByAfterSplit = orderBy.Split(',');

            //apply each oderby clause in reverse order - otherwise the 
            //IQueryable will be ordered in the wrong order
            foreach (var orderByClause in orderByAfterSplit)
            {
                //trin the order by clause, as it might contain leading spaces 
                // or trailing spaces. Cant trim the var in foreach,
                //do use another var
                var trimmedOrderByClause = orderByClause.Trim();

                //if the sort option ends with desc then order
                var orderDescending = trimmedOrderByClause.EndsWith(" desc");

                //remove " asc" or " desc" from the orderBy clause
                //get the property name to look for in the mapping dictionary
                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);

                //check if the mapping dictionary contains the key in the mapping name
                if(!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentNullException($"Key mapping for {propertyName} is missing");
                }

                //get the PropertyMappingValue
                var propertyMappingValue = mappingDictionary[propertyName];

                if(propertyMappingValue == null)
                {
                    throw new ArgumentNullException(nameof(propertyMappingValue)); 
                }

                //Run through the property names
                //so the orderby clauses are supplied in the correct order
                foreach (var destinationProperty in propertyMappingValue.DestinationProperties)
                {
                    //revert sort order if necessary
                    if(propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }

                    orderByString = orderByString +
                        (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ")
                        + destinationProperty
                        + (orderDescending ? " descending" : " ascending");

                }
            }

            return source.OrderBy(orderByString);

        }
    }
}
