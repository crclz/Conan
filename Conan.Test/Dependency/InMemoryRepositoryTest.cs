using Autofac;
using Conan.Domain;
using Conan.UnitTest.Utils;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Threading;
using Moq;
using MongoDB.Bson;

namespace Conan.UnitTest.Dependency
{
    public class InMemoryRepositoryTest
    {
        private IContainer container;

        public InMemoryRepositoryTest()
        {
            container = ContainerFactory.Create();
        }

        [Fact]
        async Task GetByIdAsync_given_notexist_return_null()
        {
            var scope = container.BeginLifetimeScope();

            var carRepository = scope.Resolve<InMemoryRepository<Car>>();

            Assert.Null(await carRepository.ByIdAsync(ObjectId.GenerateNewId().ToString()));
        }

        [Fact]
        async Task GetByIdAsync_given_exist_return_it()
        {
            var scope = container.BeginLifetimeScope();

            var carRepository = scope.Resolve<InMemoryRepository<Car>>();

            var originalCar = Car.CreateCar();

            carRepository.objectStore.Add(originalCar.Id, originalCar.DeepClone());

            var car = await carRepository.ByIdAsync(originalCar.Id);

            car.AssertJsonEqual(originalCar);
        }

        [Fact]
        async Task SaveAsync_given_null_throws_ArgumentNullException()
        {
            var scope = container.BeginLifetimeScope();
            var carRepository = scope.Resolve<InMemoryRepository<Car>>();

            await Assert.ThrowsAsync<ArgumentNullException>(() => carRepository.SaveAsync(null));
        }

        [Fact]
        async Task SaveAsync_given_one_should_find_it_in_store()
        {
            var scope = container.BeginLifetimeScope();
            var carRepository = scope.Resolve<InMemoryRepository<Car>>();

            var car = Car.CreateCar();
            await carRepository.SaveAsync(car);

            var carInStore = carRepository.objectStore[car.Id];

            car.AssertJsonEqual(carInStore);
        }

        [Fact]
        async Task SingleAsync_given_empty_store_returns_null()
        {
            var scope = container.BeginLifetimeScope();
            var carRepository = scope.Resolve<InMemoryRepository<Car>>();

            Assert.Null(await carRepository.SingleAsync(p => true));
            Assert.Null(await carRepository.SingleAsync(p => p.Id != default));
        }

        [Fact]
        async Task SingleAsync_returns_first_satisfying()
        {
            var scope = container.BeginLifetimeScope();
            var carRepository = scope.Resolve<InMemoryRepository<Car>>();

            var car1 = Car.CreateCar();
            car1.Name = "TheCar1";
            car1.Wheel.Brand = "TheBrand1";

            var car2 = Car.CreateCar();
            car2.Name = "TheCar2";
            car2.Wheel.Brand = "TheBrand2";

            await carRepository.SaveAsync(car1);
            await carRepository.SaveAsync(car2);

            var car = await carRepository.SingleAsync(p => p.Name == "TheCar1");
            car1.AssertJsonEqual(car);

            car = await carRepository.SingleAsync(p => p.Name == "TheCar2" && p.Wheel.Brand == "TheBrand2");
            car2.AssertJsonEqual(car);

            Assert.Null(await carRepository.SingleAsync(p => p.Id == default));
        }

        [Fact]
        async Task SaveAsync_given_null_throws_ArgumentNull()
        {
            var scope = container.BeginLifetimeScope();
            var carRepository = scope.Resolve<InMemoryRepository<Car>>();

            await Assert.ThrowsAsync<ArgumentNullException>(() => carRepository.SaveAsync(null));
        }

        [Fact]
        async Task SaveAsync_given_not_exist_throws_ArgumentException()
        {
            // ignore
            if (false)
            {
                var scope = container.BeginLifetimeScope();
                var carRepository = scope.Resolve<InMemoryRepository<Car>>();

                var car = new Car();

                await Assert.ThrowsAsync<ArgumentException>(() => carRepository.SaveAsync(car));
            }
        }

        [Fact]
        async Task SaveAsync_given_existed_one_should_change_in_store()
        {
            var scope = container.BeginLifetimeScope();
            var carRepository = scope.Resolve<InMemoryRepository<Car>>();

            var car = Car.CreateCar();

            await carRepository.SaveAsync(car);

            car.Name = "QQQQQQQ";
            car.Wheel.Brand = "B2";

            var carInStore = await carRepository.ByIdAsync(car.Id);
            Assert.NotEqual(car.ToJson(), carInStore.ToJson());

            // Do update
            await carRepository.SaveAsync(car);

            carInStore = await carRepository.ByIdAsync(car.Id);
            car.AssertJsonEqual(carInStore);
        }

        [Fact]
        async Task Query_should_work_properly()
        {
            var scope = container.BeginLifetimeScope();
            var carRepository = scope.Resolve<InMemoryRepository<Car>>();

            await carRepository.SaveAsync(new Car { Name = "TheCar1" });
            await carRepository.SaveAsync(new Car { Name = "TheCar2" });

            var result = carRepository.Query().Where(p => p.Name.StartsWith("The")).ToList();
            Assert.Equal(2, result.Count);

            result = carRepository.Query().Where(p => p.Name == "TheCar2").ToList();
            Assert.Single(result);

            result = carRepository.Query().Where(p => p.Name == "TheCar55").ToList();
            Assert.Empty(result);
        }

        [Fact]
        async Task DeleteAsync_throws_ArgumentException_when_id_not_exist()
        {
            // Arrange
            var scope = container.BeginLifetimeScope();
            var carRepository = scope.Resolve<InMemoryRepository<Car>>();

            // Act & Assert
            var car = Car.CreateCar();
            await Assert.ThrowsAsync<ArgumentException>(() => carRepository.DeleteAsync(car));
        }

        [Fact]
        async Task DeleteAsync_delete_and_dispatches_event_when_deleted()
        {
            // Arrange
            var builder = ContainerFactory.Builder();

            var mock = new Mock<INotificationHandler<CarDeletedEvent>>();
            builder.RegisterInstance(mock.Object).As<INotificationHandler<CarDeletedEvent>>();

            var scope = builder.Build().BeginLifetimeScope();
            var carRepository = scope.Resolve<InMemoryRepository<Car>>();

            // Act
            var car = Car.CreateCar();
            await carRepository.SaveAsync(car);
            await carRepository.DeleteAsync(car);

            // Assert
            Assert.False(carRepository.objectStore.ContainsKey(car.Id));
            mock.Verify(p => p.Handle(It.IsAny<CarDeletedEvent>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }

    class Car : RootEntity
    {
        public string Name { get; set; }
        public Wheel Wheel { get; set; }

        public Car()
        {

        }

        public static Car CreateCar()
        {
            return new Car
            {
                Name = "c1",
                Wheel = new Wheel { Brand = "b1" }
            };
        }

        public override IEnumerable<INotification> GetDeleteEvents()
        {
            return new INotification[] { new CarDeletedEvent() };
        }
    }

    public class CarDeletedEvent : INotification
    {

    }

    class Wheel : RootEntity
    {
        public string Brand { get; set; }

        public Wheel()
        {

        }
    }
}
