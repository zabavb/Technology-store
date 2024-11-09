using Client.Models.Orders;
using Client.Models.Products;
using Client.Models.Users;
using Library.Models;

namespace Client.Models
{
    public static class ModelExtension
    {
        public static User ToUser(RegisterViewModel model) =>
            new User() {
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age,
                Email = model.Email,
                Phone = model.Phone,
                Password = model.Password,
                Role = RoleType.User.ToString()
            };

        public static User ToUser(ManageUserViewModel model) =>
            new User(
                model.Id, model.Username, model.FirstName,
                model.LastName, model.Age, model.Email, model.Phone,
                model.Password, model.Role, model.Basket
            );

        public static Product ToProduct(ManageProductViewModel model) =>
            new Product(model.Id, model.Brand, model.Model, model.Producer, model.Price, model.Details);

        public static Order ToOrder(OrderViewModel model, User receiver) =>
            new Order(model.Id, model.Items, receiver, model.Country, model.Locality, model.Address, model.DeliveryDate);
    }
}
