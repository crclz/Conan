using Ardalis.GuardClauses;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Conan.Domain
{
    public static class GuardExtension
    {
        public static void BsonObjectId(this IGuardClause guardClause, string input, string parameterName)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (!ObjectId.TryParse(input, out ObjectId _))
            {
                throw new ArgumentException(
                    $"Parameter {parameterName} cannot be parsed to ObjectId: {input}",
                    parameterName);
            }
        }

        public static void NullMember<T>(this IGuardClause guardClause, IEnumerable<T> items, string parameterName)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                if (item == null)
                    throw new ArgumentException($"{parameterName} has null member", parameterName);
            }
        }

        public static void NullOrWhiteSpaceMember(
            this IGuardClause guardClause, IEnumerable<string> items, string parameterName)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                if (string.IsNullOrWhiteSpace(item))
                    throw new ArgumentException($"{parameterName} has null or white space member", parameterName);
            }
        }
    }
}
