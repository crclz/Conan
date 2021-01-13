using Conan.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Conan.UnitTest.Utils
{
    public class EntitySerializationTest
    {
        [Fact]
        void should_be_self_contained()
        {
            var person = new Person("chr");
            person.UpdateBankCard(new BankCard("1123"));

            var personJson = person.ToJson();

            var personCloned = personJson.FromJson<Person>();

            var personClonedJson = personCloned.ToJson();

            Assert.Equal(personJson, personClonedJson);


            Assert.Equal(person.Id, personCloned.Id);
            Assert.Equal(person.CreatedAt, personCloned.CreatedAt);
            Assert.Equal(person.UpdatedAt, personCloned.UpdatedAt);
            Assert.Equal(person.Name, personCloned.Name);

            Assert.Equal(person.BankCard.Id, personCloned.BankCard.Id);
            Assert.Equal(person.BankCard.Number, personCloned.BankCard.Number);
            Assert.Equal(person.BankCard.CreatedAt, personCloned.BankCard.CreatedAt);
            Assert.Equal(person.BankCard.UpdatedAt, personCloned.BankCard.UpdatedAt);
        }
    }

    class Person : RootEntity
    {
        public string Name { get; private set; }
        public BankCard BankCard { get; private set; }

        public Person()
        {

        }

        public Person(string name) : base()
        {
            SetName(name);
        }

        public void SetName(string name)
        {
            //Guard.Argument(() => name).NotNull().MinLength(1).MaxLength(5);
            Name = name;

            UpdatedAtNow();
        }

        public void UpdateBankCard(BankCard bankCard)
        {
            BankCard = bankCard;

            UpdatedAtNow();
        }
    }

    class BankCard : RootEntity
    {
        public string Number { get; private set; }

        public BankCard()
        {

        }

        public BankCard(string number) : base()
        {
            SetNumber(number);
        }

        public void SetNumber(string number)
        {
            if (number == null)
                throw new ArgumentNullException(nameof(number));
            Number = number;

            UpdatedAtNow();
        }
    }
}
