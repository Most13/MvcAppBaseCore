﻿using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using BaseApp.Common.Extensions;
using BaseApp.Common.Utils;

namespace BaseApp.Web.Code.Mappers
{
    public static class MapperExtensions
    {
        public static IMappingExpression<TSource, TDest> Map<TSource, TDest, TMember>(this IMappingExpression<TSource, TDest> expression, Expression<Func<TDest, object>> propertyDest, Expression<Func<TSource, TMember>> propertySrc)
        {
            expression.ForMember(propertyDest, opt => opt.MapFrom(propertySrc));
            return expression;
        }

        public static IMappingExpression<TSource, TDest> Ignore<TSource, TDest>(this IMappingExpression<TSource, TDest> expression, Expression<Func<TDest, object>> property)
        {
            expression.ForMember(property, opt => opt.Ignore());
            return expression;
        }

        public static IMappingExpression<TSource, TDest> IgnoreSource<TSource, TDest>(this IMappingExpression<TSource, TDest> expression, Expression<Func<TSource, object>> property)
        {
            var sourceName = PropertyUtil.GetName(property);
            if (typeof(TDest).GetProperty(sourceName) != null)
            {
                expression.ForMember(sourceName, opt => opt.Ignore());
            }
            expression.ForSourceMember(property, opt => opt.DoNotValidate());
            return expression;
        }

        [Obsolete]
        public static IMappingExpression<TSource, TDest> IgnoreAll<TSource, TDest>(this IMappingExpression<TSource, TDest> expression)
        {
            throw new Exception("Not supported, please use mapper with option MemberList.Source for such cases");
        }

        public static IMappingExpression<TSource, TDest> IgnoreAllUnmappedComplexTypes<TSource, TDest>(this IMappingExpression<TSource, TDest> expression)
        {
            var destType = typeof(TDest);
            var sourceType = typeof(TSource);

            expression.ForAllOtherMembers(
                delegate(IMemberConfigurationExpression<TSource, TDest, object> configurationExpression)
                {
                    var propName = configurationExpression.DestinationMember.Name;
                    var sourceProp = sourceType.GetProperties().Any(x => x.Name.EqualsIgnoreCase(propName));
                    if (sourceProp)
                        return;

                    var propType = destType.GetProperties().First(x => x.Name.EqualsIgnoreCase(propName)).PropertyType;

                    if (!propType.IsValueType
                        && !propType.IsEnum
                        && propType != typeof(string)
                        && propType != typeof(char[]))
                    {
                        configurationExpression.Ignore();
                    }
                });
            
            return expression;
        }

    }
}