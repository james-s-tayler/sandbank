using System;
using System.Reflection.Metadata;
using Core;
using Domain;
using Domain.User;
using NUnit.Framework;
using SlxLuhnLibrary;

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

        [Test]
        public void TestLuhn()
        {
            var accountNumber = "000001";
            var accNumberWithLuhn = ClsLuhnLibrary.WithLuhn_Base10(accountNumber);
            Assert.That(accNumberWithLuhn.EndsWith("8"));

            var isValid = ClsLuhnLibrary.CheckLuhn_Base10(accNumberWithLuhn);
            Assert.That(isValid);
        }
    }
}