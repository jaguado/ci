/****************************** Module Header ******************************\
Module Name:  DeepCloneHelper.cs
Project:      CSDeepCloneObject
Copyright (c) Microsoft Corporation.

The class contains the methods that implement deep clone using reflection.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace JAM.IIS.Test
{
    public static class DeepCloneHelper
    {
        /// <summary>
        /// Copies all public properties from one class to another.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <exception cref="System.Exception">Source and destination cannot be null and must be 
        /// of the same type!</exception>
        public static void MapProperties<T>(this T source, T destination) where T : class
        {
            // Source and destination must exist.
            if ((source == null || destination == null)) return;

            // Get properties
            var propertyInfos = source.GetType().GetProperties();
            if (!propertyInfos.Any()) return;

            // Process only public properties
            foreach (var propInfo in propertyInfos.Where(x => x.CanWrite))
            {
                // Get value from source and assign to destination.
                var value = propInfo.GetValue(source, null);
                if (value == null) continue;

                // Evaluate
                var valType = value.GetType();
                
                // Collections
                if (IsCollectionType(valType))
                {
                    var sourceCollection = value as IList;
                    if (sourceCollection == null) continue;

                    // Create new instance of collection
                    IList destinationCollection = null;
                    destinationCollection = (valType.BaseType == typeof(Array))
                        ? Array.CreateInstance(valType.GetElementType(), sourceCollection.Count)
                        : (IList)Activator.CreateInstance(valType, null);
                    if (destinationCollection == null) continue;

                    // Map properties
                    foreach (var item in sourceCollection)
                    {
                        // New item instance
                        var newItem = HasDefaultConstructor(item.GetType())
                            ? Activator.CreateInstance(item.GetType(), null)
                            : item;

                        // Map properties
                        item.MapProperties(newItem);

                        // Add to destination collection
                        if (valType.BaseType == typeof(Array))
                        {
                            destinationCollection[sourceCollection.IndexOf(item)] = newItem;
                        }
                        else
                        {
                            destinationCollection.Add(newItem);
                        }
                    }

                    // Add new collection to destination
                    propInfo.SetValue(destination, destinationCollection, null);
                }
                else
                {
                    propInfo.SetValue(destination, value, null);
                }

                // Check for properties and propagate if they exist.
                var newPropInfos = value.GetType().GetProperties();
                if (!newPropInfos.Any()) continue;

                // Copy properties for each child where necessary.
                var childSource = source.GetType().GetProperty(propInfo.Name);
                var childDestination = destination.GetType().GetProperty(propInfo.Name);
                childSource.MapProperties(childDestination);
            }
        }

        /// <summary>
        /// Determines whether the type has a default contructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static bool HasDefaultConstructor(Type type)
        {
            return
                type.GetConstructor(Type.EmptyTypes) != null ||
                type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                    .Any(x => x.GetParameters().All(p => p.IsOptional));
        }

        static bool IsCollectionType(Type type)
        {
            return (type.GetInterface("ICollection") != null);
        }
    }
}
