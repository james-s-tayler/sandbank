using System;
using System.Reflection.Metadata;
using Core;
using Domain;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var user = new User();
            var userBaseType = user.GetType().BaseType;
            var rawDomainEntity = typeof(DomainEntity<>);
            if (userBaseType.IsConstructedGenericType && userBaseType.GenericTypeArguments.Length == 1)
            {
                var constructedType = rawDomainEntity.MakeGenericType(userBaseType.GenericTypeArguments[0]);
                if (constructedType.IsInstanceOfType(user))
                {
                    Assert.Pass();
                }
            }
            
            Assert.Fail();
        }
    }
}